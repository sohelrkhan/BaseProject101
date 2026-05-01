import { AfterViewInit, Component } from '@angular/core';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { FooterComponent } from './footer/footer.component';
import { RouterOutlet } from '@angular/router';
import { UserContextService } from '../../shared/user-context.service';
import { UserModel } from '../../../api/base-api';
import { IdentityService } from '../../../identity/identity-shared/identity.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'],
  standalone: true,
  imports: [HeaderComponent, SidebarComponent, FooterComponent, RouterOutlet],
  providers: [IdentityService],
})
export class LayoutComponent implements AfterViewInit {
  constructor(
    private identityService: IdentityService,
    private userContext: UserContextService,
  ) {}

  ngOnInit(): void {
    this.identityService.getLoginInfo().subscribe((user: UserModel) => {
      this.userContext.setUser(user);
    });
  }

  ngAfterViewInit(): void {}
}
