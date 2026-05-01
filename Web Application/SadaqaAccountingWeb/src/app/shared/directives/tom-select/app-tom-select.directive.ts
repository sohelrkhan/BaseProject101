import { Directive, ElementRef, EventEmitter, Input, NgZone, OnInit, Output } from "@angular/core";
import TomSelect from "tom-select";

@Directive({
  selector: "[appTomSelect]"
})
export class TomSelectDirective implements OnInit {
  @Input() options: any[] = [];
  @Output() selectionChange = new EventEmitter<number[]>();

  private tomSelectInstance!: TomSelect;

  constructor(
    private el: ElementRef,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.ngZone.runOutsideAngular(() => {
      this.tomSelectInstance = new TomSelect(this.el.nativeElement, {
        plugins: ["remove_button"],
        maxItems: 3,
        allowEmptyOption: true,
        create: false,
        onChange: (values: any) => {
          this.ngZone.run(() => {
            this.selectionChange.emit(values.map(Number));
          });
        }
      });

      // Populate initial options
      this.options.forEach((opt) => {
        this.tomSelectInstance.addOption({ value: opt, text: opt.toString() });
        this.tomSelectInstance.addItem(opt.toString());
      });
    });
  }
}
