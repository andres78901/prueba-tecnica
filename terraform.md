# Proyecto Terraform (solo contenedores) para desplegar en **Azure** y **AWS**

Este documento contiene un **proyecto Terraform** “multi‑cloud” (Azure + AWS) para desplegar **tu contenedor** y exponerlo por HTTP. No crea base de datos administrada.

> Nota: ejecutar la BD “dentro del mismo contenedor” **no es recomendable** para producción (persistencia/backup/HA), pero este blueprint.

---

## Arquitectura

- **Azure**: **Azure Container Apps** (app)
- **AWS**: **ECS Fargate** (service) + **Application Load Balancer** (HTTP)
- La app escucha en **`8080`** (por `ASPNETCORE_URLS=http://+:8080`).

---

## Estructura del repo

```text
infra/
  README.md
  versions.tf
  providers.tf

  modules/
    azure_container_app/
      main.tf
      variables.tf
      outputs.tf
    aws_ecs_fargate/
      main.tf
      variables.tf
      outputs.tf

  envs/
    dev/
      main.tf
      variables.tf
      dev.tfvars
```

---

## Archivos raíz

### `infra/versions.tf`

```hcl
terraform {
  required_version = ">= 1.6.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}
```

### `infra/providers.tf`

```hcl
provider "azurerm" {
  features {}
}

provider "aws" {}
```

---

## Módulo Azure — Container Apps

### `infra/modules/azure_container_app/variables.tf`

```hcl
variable "location" { type = string }
variable "resource_group_name" { type = string }
variable "name" { type = string } # prefijo

variable "container_image" { type = string } # <acr>.azurecr.io/rickandmorty-api:1.0.0
variable "container_port" { type = number  default = 8080 }
variable "cpu" { type = number default = 0.5 }
variable "memory" { type = string default = "1Gi" }

variable "env" {
  type    = map(string)
  default = {}
}
```

### `infra/modules/azure_container_app/main.tf`

```hcl
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.name}-law"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "cae" {
  name                       = "${var.name}-cae"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
}

resource "azurerm_container_app" "app" {
  name                         = "${var.name}-app"
  resource_group_name          = azurerm_resource_group.rg.name
  container_app_environment_id = azurerm_container_app_environment.cae.id
  revision_mode                = "Single"

  template {
    container {
      name   = "api"
      image  = var.container_image
      cpu    = var.cpu
      memory = var.memory

      dynamic "env" {
        for_each = var.env
        content {
          name  = env.key
          value = env.value
        }
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = var.container_port
    transport        = "http"

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }
}
```

### `infra/modules/azure_container_app/outputs.tf`

```hcl
output "fqdn" {
  value = azurerm_container_app.app.ingress[0].fqdn
}
```

---

## Módulo AWS — ECS Fargate + ALB

### `infra/modules/aws_ecs_fargate/variables.tf`

```hcl
variable "name" { type = string }
variable "region" { type = string }

variable "container_image" { type = string } # <acct>.dkr.ecr.<region>.amazonaws.com/rickandmorty-api:1.0.0
variable "container_port" { type = number default = 8080 }

variable "cpu" { type = number default = 256 }
variable "memory" { type = number default = 512 }

variable "env" { type = map(string) default = {} }
```

### `infra/modules/aws_ecs_fargate/main.tf`

```hcl
data "aws_availability_zones" "azs" {}

resource "aws_vpc" "vpc" {
  cidr_block = "10.10.0.0/16"
}

resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.vpc.id
}

resource "aws_subnet" "public" {
  count                   = 2
  vpc_id                  = aws_vpc.vpc.id
  cidr_block              = cidrsubnet("10.10.0.0/16", 8, count.index)
  availability_zone       = data.aws_availability_zones.azs.names[count.index]
  map_public_ip_on_launch = true
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.vpc.id
}

resource "aws_route" "default" {
  route_table_id         = aws_route_table.public.id
  destination_cidr_block = "0.0.0.0/0"
  gateway_id             = aws_internet_gateway.igw.id
}

resource "aws_route_table_association" "public" {
  count          = 2
  subnet_id      = aws_subnet.public[count.index].id
  route_table_id = aws_route_table.public.id
}

resource "aws_security_group" "alb_sg" {
  name   = "${var.name}-alb-sg"
  vpc_id = aws_vpc.vpc.id

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_security_group" "task_sg" {
  name   = "${var.name}-task-sg"
  vpc_id = aws_vpc.vpc.id

  ingress {
    from_port       = var.container_port
    to_port         = var.container_port
    protocol        = "tcp"
    security_groups = [aws_security_group.alb_sg.id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_lb" "alb" {
  name               = "${var.name}-alb"
  load_balancer_type = "application"
  subnets            = aws_subnet.public[*].id
  security_groups    = [aws_security_group.alb_sg.id]
}

resource "aws_lb_target_group" "tg" {
  name        = "${var.name}-tg"
  port        = var.container_port
  protocol    = "HTTP"
  vpc_id      = aws_vpc.vpc.id
  target_type = "ip"

  health_check {
    path = "/health"
  }
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.alb.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.tg.arn
  }
}

resource "aws_ecs_cluster" "cluster" {
  name = "${var.name}-cluster"
}

resource "aws_iam_role" "task_exec" {
  name = "${var.name}-task-exec"
  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [{
      Effect = "Allow",
      Principal = { Service = "ecs-tasks.amazonaws.com" },
      Action = "sts:AssumeRole"
    }]
  })
}

resource "aws_iam_role_policy_attachment" "task_exec_attach" {
  role       = aws_iam_role.task_exec.name
  policy_arn  = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_cloudwatch_log_group" "lg" {
  name              = "/ecs/${var.name}"
  retention_in_days = 30
}

resource "aws_ecs_task_definition" "task" {
  family                   = var.name
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = tostring(var.cpu)
  memory                   = tostring(var.memory)
  execution_role_arn       = aws_iam_role.task_exec.arn

  container_definitions = jsonencode([{
    name  = "api",
    image = var.container_image,
    portMappings = [{
      containerPort = var.container_port,
      protocol      = "tcp"
    }],
    environment = [for k, v in var.env : { name = k, value = v }],
    logConfiguration = {
      logDriver = "awslogs",
      options = {
        awslogs-group         = aws_cloudwatch_log_group.lg.name,
        awslogs-region        = var.region,
        awslogs-stream-prefix = "api"
      }
    }
  }])
}

resource "aws_ecs_service" "svc" {
  name            = "${var.name}-svc"
  cluster         = aws_ecs_cluster.cluster.id
  task_definition = aws_ecs_task_definition.task.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets           = aws_subnet.public[*].id
    security_groups   = [aws_security_group.task_sg.id]
    assign_public_ip  = true
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.tg.arn
    container_name   = "api"
    container_port   = var.container_port
  }

  depends_on = [aws_lb_listener.http]
}
```

### `infra/modules/aws_ecs_fargate/outputs.tf`

```hcl
output "alb_dns_name" {
  value = aws_lb.alb.dns_name
}
```

---

## Entorno `dev`

### `infra/envs/dev/variables.tf`

```hcl
variable "azure_location" { type = string }
variable "azure_rg" { type = string }
variable "azure_container_image" { type = string }

variable "aws_region" { type = string }
variable "aws_container_image" { type = string }
```

### `infra/envs/dev/main.tf`

```hcl
module "azure" {
  source              = "../../modules/azure_container_app"
  location            = var.azure_location
  resource_group_name = var.azure_rg
  name                = "rickmorty-dev"
  container_image     = var.azure_container_image

  env = {
    ASPNETCORE_ENVIRONMENT   = "Production"
    RickAndMortyApi__BaseUrl = "https://rickandmortyapi.com/api/"
  }
}

module "aws" {
  source          = "../../modules/aws_ecs_fargate"
  name            = "rickmorty-dev"
  region          = var.aws_region
  container_image = var.aws_container_image

  env = {
    ASPNETCORE_ENVIRONMENT   = "Production"
    RickAndMortyApi__BaseUrl = "https://rickandmortyapi.com/api/"
  }
}

output "azure_url" {
  value = "https://${module.azure.fqdn}"
}

output "aws_url" {
  value = "http://${module.aws.alb_dns_name}"
}
```

### `infra/envs/dev/dev.tfvars` (ejemplo)

```hcl
azure_location        = "eastus"
azure_rg              = "rg-rickmorty-dev"
azure_container_image = "<TU_ACR>.azurecr.io/rickandmorty-api:1.0.0"

aws_region          = "us-east-1"
aws_container_image = "<TU_AWS_ACCOUNT>.dkr.ecr.us-east-1.amazonaws.com/rickandmorty-api:1.0.0"
```

---

## Cómo ejecutar (comandos)

```bash
cd infra/envs/dev
terraform init
terraform plan -var-file="dev.tfvars"
terraform apply -var-file="dev.tfvars"
```

Validación:

- Azure: `https://<fqdn>/health`
- AWS: `http://<alb_dns>/health`

---

## CI/CD (recomendación mínima)

1. Build imagen y push a:
   - **Azure ACR**
   - **AWS ECR**
2. Actualiza el tag de `container_image` en `dev.tfvars` (o usa variables en el pipeline).
3. Ejecuta `terraform apply`.

---

## Nota sobre persistencia “BD dentro del contenedor”

Si realmente guardas datos en disco dentro del contenedor, al reiniciar puedes perderlos. Para persistencia “sin BD administrada” necesitarías:

- **Azure**: Azure Files montado como volumen en Container Apps
- **AWS**: EFS montado en Fargate

Si quieres esa variante, se puede extender este proyecto con módulos para Azure Files / EFS.

