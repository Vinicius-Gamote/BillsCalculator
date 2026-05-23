import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AuthResponse,
  Category,
  CreateCategoryRequest,
  CreateTransactionRequest,
  Dashboard,
  DashboardFilters,
  FinancialTransaction,
  TransactionFilters,
  TransactionType
} from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  register(payload: { name: string; email: string; password: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/register`, payload);
  }

  login(payload: { email: string; password: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/login`, payload);
  }

  getCategories(type?: TransactionType | ''): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/categories`, {
      params: this.toParams({ type })
    });
  }

  createCategory(payload: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(`${this.baseUrl}/categories`, payload);
  }

  updateCategory(id: string, payload: CreateCategoryRequest): Observable<Category> {
    return this.http.put<Category>(`${this.baseUrl}/categories/${id}`, payload);
  }

  archiveCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/categories/${id}`);
  }

  getTransactions(filters: TransactionFilters = {}): Observable<FinancialTransaction[]> {
    return this.http.get<FinancialTransaction[]>(`${this.baseUrl}/transactions`, {
      params: this.toParams(filters)
    });
  }

  createTransaction(payload: CreateTransactionRequest): Observable<FinancialTransaction> {
    return this.http.post<FinancialTransaction>(`${this.baseUrl}/transactions`, payload);
  }

  updateTransaction(id: string, payload: CreateTransactionRequest): Observable<FinancialTransaction> {
    return this.http.put<FinancialTransaction>(`${this.baseUrl}/transactions/${id}`, payload);
  }

  deleteTransaction(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/transactions/${id}`);
  }

  getDashboard(filters: DashboardFilters): Observable<Dashboard> {
    return this.http.get<Dashboard>(`${this.baseUrl}/dashboard`, {
      params: this.toParams(filters)
    });
  }

  private toParams(values: object): HttpParams {
    let params = new HttpParams();

    Object.entries(values).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    });

    return params;
  }
}
