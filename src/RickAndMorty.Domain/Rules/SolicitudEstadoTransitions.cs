using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Domain.Rules;

public static class SolicitudEstadoTransitions
{
    public static bool IsValid(SolicitudEstado desde, SolicitudEstado hacia) =>
        (desde, hacia) switch
        {
            (SolicitudEstado.Pendiente, SolicitudEstado.EnProceso) => true,
            (SolicitudEstado.Pendiente, SolicitudEstado.Rechazada) => true,
            (SolicitudEstado.EnProceso, SolicitudEstado.Aprobada) => true,
            (SolicitudEstado.EnProceso, SolicitudEstado.Rechazada) => true,
            _ => false,
        };
}
