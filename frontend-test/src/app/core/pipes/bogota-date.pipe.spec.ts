import { BogotaDatePipe } from './bogota-date.pipe';

describe('BogotaDatePipe', () => {
  const pipe = new BogotaDatePipe();

  it('formats ISO to dd/MM/yyyy HH:mm:ss in America/Bogota', () => {
    const iso = '2024-03-15T18:30:45Z';
    const formatted = pipe.transform(iso);
    expect(formatted).toMatch(/^\d{2}\/\d{2}\/\d{4} \d{2}:\d{2}:\d{2}$/);
  });
});
