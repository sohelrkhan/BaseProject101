import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-access-denied',
  templateUrl: './access-denied.component.html',
  styleUrls: ['./access-denied.component.css'],
  standalone: true,
  imports: [RouterLink]
})
export class AccessDeniedComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
