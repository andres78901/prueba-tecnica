export function formatBogotaDate(value: string | Date | null | undefined): string {
  if (!value) return '';
  const d = typeof value === 'string' ? new Date(value) : value;
  if (isNaN(d.getTime())) return '';

  const parts = new Intl.DateTimeFormat('es-CO', {
    timeZone: 'America/Bogota',
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: false
  }).formatToParts(d as Date);

  const map: Record<string, string> = {};
  for (const p of parts) if (p.type !== 'literal') map[p.type] = p.value;

  const day = map['day'] || '00';
  const month = map['month'] || '00';
  const year = map['year'] || '0000';
  const hour = map['hour'] || '00';
  const minute = map['minute'] || '00';
  const second = map['second'] || '00';
  return `${day}/${month}/${year} ${hour}:${minute}:${second}`;
}

