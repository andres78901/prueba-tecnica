import { Pipe, PipeTransform } from '@angular/core';
import { formatBogotaDate } from '../util/format-bogota-date';

@Pipe({ name: 'bogotaDate', standalone: true })
export class BogotaDatePipe implements PipeTransform {
  transform(value: string | Date | null | undefined): string {
    return formatBogotaDate(value);
  }
}
