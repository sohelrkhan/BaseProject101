import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { FeatureService, FeatureWorkflowProcessViewModel, WorkflowProcessCreateModel, WorkflowProcessService, WorkflowProcessViewModel } from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { SelectModel } from '../../../../../../shared/models/select-model';

@Component({
  selector: 'app-feature-workflow-list',
  templateUrl: './feature-workflow-list.component.html',
  styleUrls: ['./feature-workflow-list.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule],
  providers:[FeatureService, WorkflowProcessService]
})
export class FeatureWorkflowListComponent implements OnInit {

  @Input() featureId!: number | undefined;

  workflowList : any[] = [] ;

  constructor(private featureService: FeatureService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, private router: Router,
    private activatedRoute: ActivatedRoute, private workflowProcessService: WorkflowProcessService) { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['featureId'] && this.featureId) {

    }
  }

}
