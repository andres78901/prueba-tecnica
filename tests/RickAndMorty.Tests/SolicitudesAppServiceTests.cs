using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Application.Common.Exceptions;
using RickAndMorty.Application.Dtos.Solicitudes;
using RickAndMorty.Application.Services;
using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Tests;

public sealed class SolicitudesAppServiceTests
{
    [Fact]
    public async Task CrearAsync_cuando_existe_personaje_inserta_solicitud()
    {
        var personajeRepo = new Mock<IPersonajeRepository>();
        personajeRepo
            .Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Personaje { Id = 10, Nombre = "Morty" });

        var solicitudRepo = new Mock<ISolicitudRepository>();
        solicitudRepo
            .Setup(x => x.AddAsync(It.IsAny<Solicitud>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (Solicitud s, CancellationToken _) =>
                {
                    s.Id = 500;
                    return s;
                });

        var logger = new Mock<ILogger<SolicitudesAppService>>();
        var sut = new SolicitudesAppService(solicitudRepo.Object, personajeRepo.Object, logger.Object);

        var dto = await sut.CrearAsync(
            new CrearSolicitudRequestDto
            {
                PersonajeId = 10,
                Solicitante = "Ana López",
                Evento = "Revisión documental",
                FechaEvento = DateTime.UtcNow,
            });

        dto.Id.Should().Be(500);
        dto.Estado.Should().Be(SolicitudEstado.Pendiente);
        solicitudRepo.Verify(
            x => x.AddAsync(It.Is<Solicitud>(s => s.PersonajeId == 10), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CambiarEstadoAsync_desde_estado_terminal_lanza_regla_negocio()
    {
        var solicitud = new Solicitud
        {
            Id = 1,
            PersonajeId = 2,
            Estado = SolicitudEstado.Aprobada,
            Solicitante = "x",
            Evento = "y",
            FechaEvento = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow,
        };

        var solicitudRepo = new Mock<ISolicitudRepository>();
        solicitudRepo
            .Setup(x => x.GetByIdAsync(1, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitud);

        var personajeRepo = new Mock<IPersonajeRepository>();
        var logger = new Mock<ILogger<SolicitudesAppService>>();
        var sut = new SolicitudesAppService(solicitudRepo.Object, personajeRepo.Object, logger.Object);

        var act = async () => await sut.CambiarEstadoAsync(
            1,
            new CambiarEstadoSolicitudRequestDto { Estado = SolicitudEstado.EnProceso },
            CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Transición de estado no permitida*");
    }

    [Fact]
    public async Task CrearAsync_cuando_no_existe_personaje_lanza_not_found()
    {
        var personajeRepo = new Mock<IPersonajeRepository>();
        personajeRepo
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Personaje?)null);

        var solicitudRepo = new Mock<ISolicitudRepository>();
        var logger = new Mock<ILogger<SolicitudesAppService>>();
        var sut = new SolicitudesAppService(solicitudRepo.Object, personajeRepo.Object, logger.Object);

        var act = async () => await sut.CrearAsync(
            new CrearSolicitudRequestDto
            {
                PersonajeId = 99,
                Solicitante = "Ana",
                Evento = "Evento",
                FechaEvento = DateTime.UtcNow,
            });

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*No existe personaje con id 99*");

        solicitudRepo.Verify(
            x => x.AddAsync(It.IsAny<Solicitud>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CambiarEstadoAsync_cuando_transicion_es_valida_actualiza_y_devuelve_dto()
    {
        var solicitud = new Solicitud
        {
            Id = 7,
            PersonajeId = 2,
            Estado = SolicitudEstado.Pendiente,
            Solicitante = "Ana",
            Evento = "Evento",
            FechaEvento = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow,
        };

        var personaje = new Personaje { Id = 2, Nombre = "Rick" };
        var luegoDeActualizar = new Solicitud
        {
            Id = 7,
            PersonajeId = 2,
            Personaje = personaje,
            Estado = SolicitudEstado.EnProceso,
            Solicitante = "Ana",
            Evento = "Evento",
            FechaEvento = solicitud.FechaEvento,
            FechaCreacion = solicitud.FechaCreacion,
            FechaActualizacion = DateTime.UtcNow,
        };

        var solicitudRepo = new Mock<ISolicitudRepository>();
        solicitudRepo
            .Setup(x => x.GetByIdAsync(7, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitud);
        solicitudRepo
            .Setup(x => x.GetByIdAsync(7, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(luegoDeActualizar);

        var personajeRepo = new Mock<IPersonajeRepository>();
        var logger = new Mock<ILogger<SolicitudesAppService>>();
        var sut = new SolicitudesAppService(solicitudRepo.Object, personajeRepo.Object, logger.Object);

        var dto = await sut.CambiarEstadoAsync(
            7,
            new CambiarEstadoSolicitudRequestDto { Estado = SolicitudEstado.EnProceso },
            CancellationToken.None);

        dto.Estado.Should().Be(SolicitudEstado.EnProceso);
        dto.PersonajeNombre.Should().Be("Rick");

        solicitudRepo.Verify(
            x => x.UpdateAsync(It.Is<Solicitud>(s => s.Id == 7 && s.Estado == SolicitudEstado.EnProceso),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
