import { NgFor, NgIf, NgStyle } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../core/api.service';
import {
  Category,
  CreateTransactionRequest,
  FinancialTransaction,
  TransactionFilters,
  TransactionType
} from '../../core/models';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, NgStyle, ReactiveFormsModule],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.css'
})
export class TransactionsComponent implements OnInit {
  categories: Category[] = [];
  transactions: FinancialTransaction[] = [];
  editingId = '';
  loading = false;
  error = '';

  filters: TransactionFilters = {
    type: '',
    categoryId: '',
    from: '',
    to: ''
  };

  readonly form = this.fb.nonNullable.group({
    description: ['', [Validators.required, Validators.maxLength(160)]],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    occurredOn: [this.today(), [Validators.required]],
    type: ['Expense' as TransactionType, [Validators.required]],
    categoryId: ['', [Validators.required]],
    notes: ['']
  });

  constructor(private readonly fb: FormBuilder, private readonly api: ApiService) {}

  ngOnInit(): void {
    this.form.controls.type.valueChanges.subscribe(() => {
      this.form.controls.categoryId.setValue('');
    });
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';

    forkJoin({
      categories: this.api.getCategories(),
      transactions: this.api.getTransactions(this.normalizedFilters())
    }).subscribe({
      next: ({ categories, transactions }) => {
        this.categories = categories;
        this.transactions = transactions;
        this.loading = false;
      },
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel carregar lancamentos.';
        this.loading = false;
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.toPayload();
    const request = this.editingId
      ? this.api.updateTransaction(this.editingId, payload)
      : this.api.createTransaction(payload);

    request.subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel salvar o lancamento.';
      }
    });
  }

  edit(transaction: FinancialTransaction): void {
    this.editingId = transaction.id;
    this.form.setValue({
      description: transaction.description,
      amount: transaction.amount,
      occurredOn: transaction.occurredOn,
      type: transaction.type,
      categoryId: transaction.categoryId,
      notes: transaction.notes ?? ''
    });
  }

  delete(transaction: FinancialTransaction): void {
    this.api.deleteTransaction(transaction.id).subscribe({
      next: () => this.load(),
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel excluir o lancamento.';
      }
    });
  }

  reset(): void {
    this.editingId = '';
    this.form.reset({
      description: '',
      amount: 0,
      occurredOn: this.today(),
      type: 'Expense',
      categoryId: '',
      notes: ''
    });
  }

  formCategories(): Category[] {
    return this.categories.filter((category) => category.type === this.form.controls.type.value);
  }

  filterCategories(): Category[] {
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

  applyTypeFilter(): void {
    this.filters.categoryId = '';
    this.load();
  }

  private toPayload(): CreateTransactionRequest {
    const value = this.form.getRawValue();
    return {
      categoryId: value.categoryId,
      type: value.type,
      description: value.description,
      amount: Number(value.amount),
      occurredOn: value.occurredOn,
      notes: value.notes || null
    };
  }

  private normalizedFilters(): TransactionFilters {
    return {
      type: this.filters.type || undefined,
      categoryId: this.filters.categoryId || undefined,
      from: this.filters.from || undefined,
      to: this.filters.to || undefined
    };
  }

  private today(): string {
    return new Date().toISOString().slice(0, 10);
  }
}
