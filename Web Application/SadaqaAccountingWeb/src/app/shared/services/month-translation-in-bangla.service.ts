import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root"
})
export class MonthTranslationInBanglaService {
  private monthMap: { [key: string]: string } = {
    January: "জানুয়ারি",
    February: "ফেব্রুয়ারি",
    March: "মার্চ",
    April: "এপ্রিল",
    May: "মে",
    June: "জুন",
    July: "জুলাই",
    August: "আগস্ট",
    September: "সেপ্টেম্বর",
    October: "অক্টোবর",
    November: "নভেম্বর",
    December: "ডিসেম্বর"
  };

  constructor() {}

  toBanglaMonth(englishMonth: string): string {
    return this.monthMap[englishMonth] || englishMonth;
  }
}
