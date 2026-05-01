import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { GroupReportRegistryModel, ReportRegistryService } from '../../../../../api/base-api';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css'],
  standalone: true,
  imports: [RouterLink, CommonModule, NgxSpinnerModule, FormsModule],
  providers: [ReportRegistryService]
})
export class ReportsComponent implements OnInit {
private readonly moduleCode: string = "RPT";
  groupReportRegistryModel: GroupReportRegistryModel[] = [];
  filteredGroupReportRegistryModel: GroupReportRegistryModel[] = [];
  searchText: string = "";

  constructor(
    private reportRegistryService: ReportRegistryService,
    private spinnerService: NgxSpinnerService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {
    this.getAllReportRegistryByModuleCode();
  }

  private getAllReportRegistryByModuleCode(): void {
    this.spinnerService.show();
    this.reportRegistryService.getAllReportRegistryByModuleCode(this.moduleCode).subscribe(
      (result: GroupReportRegistryModel[]) => {
        this.groupReportRegistryModel = result || [];
        this.filteredGroupReportRegistryModel = [...this.groupReportRegistryModel];
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  onSearchChange(): void {
    if (!this.searchText.trim()) {
      this.filteredGroupReportRegistryModel = [...this.groupReportRegistryModel];
      return;
    }

    const searchLower = this.searchText.toLowerCase();
    this.filteredGroupReportRegistryModel = this.groupReportRegistryModel
      .map((group) => {
        const filteredGroup = new GroupReportRegistryModel();
        filteredGroup.groupName = group.groupName;
        filteredGroup.reportRegistryList = group.reportRegistryList.filter(
          (report) =>
            report.name.toLowerCase().includes(searchLower) ||
            report.reportCode.toLowerCase().includes(searchLower)
        );
        return filteredGroup;
      })
      .filter((group) => group.reportRegistryList.length > 0);
  }

  highlightSearchText(text: string): SafeHtml {
    if (!this.searchText.trim()) {
      return text;
    }

    const searchRegex = new RegExp(`(${this.escapeRegex(this.searchText)})`, "gi");
    const highlighted = text.replace(searchRegex, '<mark class="highlight-search">$1</mark>');
    return this.sanitizer.bypassSecurityTrustHtml(highlighted);
  }

  private escapeRegex(text: string): string {
    return text.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
  }
}
