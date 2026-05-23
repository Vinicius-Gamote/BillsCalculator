import { NgFor, NgIf, NgStyle } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { Category, CategoryTotal, Dashboard, DashboardFilters, MonthlyTotal, TransactionType } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, NgStyle],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  readonly months = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];
  readonly years = Array.from({ length: 6 }, (_, index) => new Date().getFullYear() - index);

  filters: DashboardFilters = {
    year: new Date().getFullYear(),
    month: new Date().getMonth() + 1,
    type: '',
    categoryId: ''
  };

  categories: Category[] = [];
  dashboard?: Dashboard;
  loading = false;
  error = '';

  constructor(private readonly api: ApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';

    forkJoin({
      categories: this.api.getCategories(),
      dashboard: this.api.getDashboard(this.normalizedFilters())
    }).subscribe({
      next: ({ categories, dashboard }) => {
        this.categories = categories;
        this.dashboard = dashboard;
        this.loading = false;
      },
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel carregar o dashboard.';
        this.loading = false;
      }
    });
  }

  filteredCategories(): Category[] {
    if (!this.filters.type) {
      return this.categories;
    }

    return this.categories.filter((category) => category.type === this.filters.type);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }

  typeLabel(type: TransactionType): string {
    return type === 'Income' ? 'Entrada' : 'Saida';
  }

  maxMonthlyTotal(): number {
    if (!this.dashboard?.monthlyTotals.length) {
      return 1;
    }

    return Math.max(...this.dashboard.monthlyTotals.map((item) => Math.max(item.income, item.expense)), 1);
  }

  maxCategoryTotal(): number {
    if (!this.dashboard?.categoryTotals.length) {
      return 1;
    }

    return Math.max(...this.dashboard.categoryTotals.map((item) => item.total), 1);
  }

  barHeight(item: MonthlyTotal, type: TransactionType): number {
    const value = type === 'Income' ? item.income : item.expense;
    return Math.max((value / this.maxMonthlyTotal()) * 100, value > 0 ? 6 : 0);
  }

  categoryWidth(item: CategoryTotal): number {
    return Math.max((item.total / this.maxCategoryTotal()) * 100, item.total > 0 ? 8 : 0);
  }

  private normalizedFilters(): DashboardFilters {
    return {
      year: this.filters.year,
      month: this.filters.month,
      type: this.filters.type || undefined,
      categoryId: this.filters.categoryId || undefined
    };
  }
}
