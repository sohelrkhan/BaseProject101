import { Pipe, PipeTransform } from "@angular/core";
import { MonthTranslationInBanglaService } from "../services/month-translation-in-bangla.service";

@Pipe({
  name: "banglaMonth",
  standalone: true
})
export class BanglaMonthPipe implements PipeTransform {
  constructor(private monthService: MonthTranslationInBanglaService) {}

  transform(value: string): string {
    return this.monthService.toBanglaMonth(value);
  }
}
