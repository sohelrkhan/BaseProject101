import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "ordinaldate",
  standalone: true
})
export class OrdinalDatePipe implements PipeTransform {
  transform(value: Date | string | number): string {
    if (!value) return "";

    const date = new Date(value);
    const day = date.getDate();
    const month = date.toLocaleString("en-US", { month: "long" }); // "July"
    const year = date.getFullYear();

    return `${this.getOrdinal(day)} ${month} ${year}`;
  }

  private getOrdinal(day: number): string {
    if (day >= 11 && day <= 13) {
      return `${day}th`;
    }
    switch (day % 10) {
      case 1:
        return `${day}st`;
      case 2:
        return `${day}nd`;
      case 3:
        return `${day}rd`;
      default:
        return `${day}th`;
    }
  }
}
