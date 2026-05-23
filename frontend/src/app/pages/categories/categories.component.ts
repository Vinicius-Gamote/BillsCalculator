import { NgFor, NgIf, NgStyle } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/api.service';
import { Category, TransactionType } from '../../core/models';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [NgFor, NgIf, NgStyle, ReactiveFormsModule],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.css'
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  editingId = '';
  loading = false;
  error = '';

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(80)]],
    type: ['Expense' as TransactionType, [Validators.required]],
    color: ['#ef4444', [Validators.required]]
  });

  constructor(private readonly fb: FormBuilder, private readonly api: ApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';

    this.api.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.loading = false;
      },
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel carregar categorias.';
        this.loading = false;
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue();
    const request = this.editingId
      ? this.api.updateCategory(this.editingId, payload)
      : this.api.createCategory(payload);

    request.subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel salvar a categoria.';
      }
    });
  }

  edit(category: Category): void {
    this.editingId = category.id;
    this.form.setValue({
      name: category.name,
      type: category.type,
      color: category.color
    });
  }

  archive(category: Category): void {
    this.api.archiveCategory(category.id).subscribe({
      next: () => this.load(),
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel arquivar a categoria.';
      }
    });
  }

  reset(): void {
    this.editingId = '';
    this.form.reset({
      name: '',
      type: 'Expense',
      color: '#ef4444'
    });
  }

  typeLabel(type: TransactionType): string {
    return type === 'Income' ? 'Entrada' : 'Saida';
  }
}
