export interface UiError {
  status: number;
  message: string;
  errors?: string[] | null;
}
