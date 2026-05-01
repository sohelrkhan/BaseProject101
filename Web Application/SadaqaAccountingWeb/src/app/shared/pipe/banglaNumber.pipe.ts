import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "banglaNumber",
  standalone: true
})
export class BanglaNumberPipe implements PipeTransform {
  private engToBanglaDigits: { [key: string]: string } = {
    "0": "০",
    "1": "১",
    "2": "২",
    "3": "৩",
    "4": "৪",
    "5": "৫",
    "6": "৬",
    "7": "৭",
    "8": "৮",
    "9": "৯"
  };

  transform(value: any): string {
    if (value == null) return "";
    return value.toString().replace(/[0-9]/g, (d) => this.engToBanglaDigits[d]);
  }
}
