import { Routes } from '@angular/router';
import { DashboardComponent } from '../components/layout/dashboard/dashboard.component';
import { CompanyListComponent } from '../components/layout/dashboard/master-settings/company/company-list/company-list.component';
import { CompanyCreateComponent } from '../components/layout/dashboard/master-settings/company/company-create/company-create.component';
import { CompanyUpdateComponent } from '../components/layout/dashboard/master-settings/company/company-update/company-update.component';
import { ModuleListComponent } from '../components/layout/dashboard/master-settings/module/module-list/module-list.component';
import { ModuleCreateComponent } from '../components/layout/dashboard/master-settings/module/module-create/module-create.component';
import { ModuleUpdateComponent } from '../components/layout/dashboard/master-settings/module/module-update/module-update.component';
import { FeatureListGlobalComponent } from '../components/layout/dashboard/master-settings/feature/feature-list-global/feature-list-global.component';
import { FeatureCreateComponent } from '../components/layout/dashboard/master-settings/feature/feature-create/feature-create.component';
import { FeatureUpdateComponent } from '../components/layout/dashboard/master-settings/feature/feature-update/feature-update.component';
import { FeatureWorkflowSettingComponent } from '../components/layout/dashboard/master-settings/feature/feature-workflow-setting/feature-workflow-setting.component';
import { FeatureListComponent } from '../components/layout/dashboard/master-settings/feature/feature-list/feature-list.component';
import { UserListComponent } from '../components/layout/dashboard/master-settings/user/user-list/user-list.component';
import { UserCreateComponent } from '../components/layout/dashboard/master-settings/user/user-create/user-create.component';
import { UserUpdateComponent } from '../components/layout/dashboard/master-settings/user/user-update/user-update.component';
import { ActionListComponent } from '../components/layout/dashboard/master-settings/access-control/action/action-list/action-list.component';
import { ActionCreateComponent } from '../components/layout/dashboard/master-settings/access-control/action/action-create/action-create.component';
import { ActionUpdateComponent } from '../components/layout/dashboard/master-settings/access-control/action/action-update/action-update.component';
import { FeatureActionMappingCreateComponent } from '../components/layout/dashboard/master-settings/access-control/feature-action-mapping/feature-action-mapping-create/feature-action-mapping-create.component';
import { CreateUserAccountUnitComponent } from '../components/layout/dashboard/master-settings/access-control/user-account-unit/create-user-account-unit/create-user-account-unit.component';
import { CreateUserAccessControlComponent } from '../components/layout/dashboard/master-settings/access-control/user-access-control/create-user-access-control/create-user-access-control.component';
import { ReportRegistryComponent } from '../components/layout/dashboard/master-settings/report-access-control/report-registry/report-registry.component';
import { ReportUserAccessComponent } from '../components/layout/dashboard/master-settings/report-access-control/report-user-access/report-user-access.component';
import { EmployeeCreateComponent } from '../components/layout/dashboard/hrms/employee/employee-create/employee-create.component';
import { EmployeeUpdateComponent } from '../components/layout/dashboard/hrms/employee/employee-update/employee-update.component';
import { EmployeeListComponent } from '../components/layout/dashboard/hrms/employee/employee-list/employee-list.component';
import { EventsCreateComponent } from '../components/layout/dashboard/master-settings/events/events-create/events-create.component';
import { EventsUpdateComponent } from '../components/layout/dashboard/master-settings/events/events-update/events-update.component';
import { EventsComponent } from '../components/layout/dashboard/master-settings/events/events.component';
import { DonorCreateComponent } from '../components/layout/dashboard/donor-management/donors/donor-create/donor-create.component';
import { DonorUpdateComponent } from '../components/layout/dashboard/donor-management/donors/donor-update/donor-update.component';
import { DonorsComponent } from '../components/layout/dashboard/donor-management/donors/donors.component';
import { ExpenseCategoriesComponent } from '../components/layout/dashboard/expense-management/expense-categories/expense-categories.component';
import { ExpenseCategoryCreateComponent } from '../components/layout/dashboard/expense-management/expense-categories/expense-category-create/expense-category-create.component';
import { ExpenseCategoryUpdateComponent } from '../components/layout/dashboard/expense-management/expense-categories/expense-category-update/expense-category-update.component';
import { IncomeCategoriesComponent } from '../components/layout/dashboard/income-management/income-categories/income-categories.component';
import { IncomeCategoryCreateComponent } from '../components/layout/dashboard/income-management/income-categories/income-category-create/income-category-create.component';
import { IncomeCategoryUpdateComponent } from '../components/layout/dashboard/income-management/income-categories/income-category-update/income-category-update.component';
import { ExpensesComponent } from '../components/layout/dashboard/expense-management/expenses/expenses.component';
import { ExpenseCreateComponent } from '../components/layout/dashboard/expense-management/expenses/expense-create/expense-create.component';
import { ExpenseUpdateComponent } from '../components/layout/dashboard/expense-management/expenses/expense-update/expense-update.component';
import { BanksComponent } from '../components/layout/dashboard/cash-bank-management/banks/banks.component';
import { BankCreateComponent } from '../components/layout/dashboard/cash-bank-management/banks/bank-create/bank-create.component';
import { BankUpdateComponent } from '../components/layout/dashboard/cash-bank-management/banks/bank-update/bank-update.component';
import { IncomeCreateComponent } from '../components/layout/dashboard/income-management/incomes/income-create/income-create.component';
import { IncomeUpdateComponent } from '../components/layout/dashboard/income-management/incomes/income-update/income-update.component';
import { IncomesComponent } from '../components/layout/dashboard/income-management/incomes/incomes.component';
import { OpeningBalancesComponent } from '../components/layout/dashboard/cash-bank-management/opening-balances/opening-balances.component';
import { OpeningBalanceCreateComponent } from '../components/layout/dashboard/cash-bank-management/opening-balances/opening-balance-create/opening-balance-create.component';
import { OpeningBalanceUpdateComponent } from '../components/layout/dashboard/cash-bank-management/opening-balances/opening-balance-update/opening-balance-update.component';

export const hrmsRoutes: Routes = [
  // For dashboard
  { path: 'dashboard', component: DashboardComponent },

  // For company
  {
    path: 'companies',
    component: CompanyListComponent,
    // canActivate: [AuthGuard, PermissionGuard],
    // data: { permission: { feature: "Company", action: "List" } }
  },

  { path: 'company/create', component: CompanyCreateComponent },
  { path: 'company/update/:recordId', component: CompanyUpdateComponent },

  // For module
  { path: 'modules', component: ModuleListComponent },
  { path: 'module/create', component: ModuleCreateComponent },
  { path: 'module/update/:recordId', component: ModuleUpdateComponent },

  // For feature
  { path: 'feature-global', component: FeatureListGlobalComponent },
  { path: 'feature/create', component: FeatureCreateComponent },
  { path: 'feature/update/:recordId', component: FeatureUpdateComponent },
  { path: 'feature/workflow_setting/:recordId', component: FeatureWorkflowSettingComponent },

  // For feature
  { path: 'features', component: FeatureListComponent },
  { path: 'feature/create', component: FeatureCreateComponent },
  { path: 'feature/update/:recordId', component: FeatureUpdateComponent },

  // For user
  { path: 'users', component: UserListComponent },
  { path: 'user/create', component: UserCreateComponent },
  { path: 'user/update/:recordId', component: UserUpdateComponent },

  // For action
  { path: 'actions', component: ActionListComponent },
  { path: 'action/create', component: ActionCreateComponent },
  { path: 'action/update/:recordId', component: ActionUpdateComponent },

  // For feature action mapping
  { path: 'feature-action-mapping/create', component: FeatureActionMappingCreateComponent },

  // For user account unit mapping
  { path: 'user-account-mapping/create', component: CreateUserAccountUnitComponent },

  // For user access control
  { path: 'user-access-control/create', component: CreateUserAccessControlComponent },

  // For Report Registry
  { path: 'report-registry', component: ReportRegistryComponent },
  { path: 'report-user-access', component: ReportUserAccessComponent },

  // Employee
  { path: 'employees', component: EmployeeListComponent },
  { path: 'employee/create', component: EmployeeCreateComponent },
  { path: 'employee/update/:recordId', component: EmployeeUpdateComponent },

  // Bank
  { path: 'banks', component: BanksComponent },
  { path: 'bank/create', component: BankCreateComponent },
  { path: 'bank/update/:recordId', component: BankUpdateComponent },

  // Opening Balance
  { path: 'opening-balances', component: OpeningBalancesComponent },
  { path: 'opening-balance/create', component: OpeningBalanceCreateComponent },
  { path: 'opening-balance/update/:recordId', component: OpeningBalanceUpdateComponent },

  // Expense Category
  { path: 'expense-categories', component: ExpenseCategoriesComponent },
  { path: 'expense-category/create', component: ExpenseCategoryCreateComponent },
  { path: 'expense-category/update/:recordId', component: ExpenseCategoryUpdateComponent },

  // Expense 
  { path: 'expenses', component: ExpensesComponent },
  { path: 'expense/create', component: ExpenseCreateComponent },
  { path: 'expense/update/:recordId', component: ExpenseUpdateComponent },

  // For event
  { path: 'events', component: EventsComponent },
  { path: 'event/create', component: EventsCreateComponent },
  { path: 'event/update/:recordId', component: EventsUpdateComponent },

  // For donor
  { path: 'donors', component: DonorsComponent },
  { path: 'donor/create', component: DonorCreateComponent },
  { path: 'donor/update/:recordId', component: DonorUpdateComponent },

  // Income Category
  { path: 'income-categories', component: IncomeCategoriesComponent },
  { path: 'income-category/create', component: IncomeCategoryCreateComponent },
  { path: 'income-category/update/:recordId', component: IncomeCategoryUpdateComponent },

  // Income
  { path: 'incomes', component: IncomesComponent },
  { path: 'income/create', component: IncomeCreateComponent },
  { path: 'income/update/:recordId', component: IncomeUpdateComponent }
];