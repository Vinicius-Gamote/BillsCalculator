export type TransactionType = 'Income' | 'Expense';

export interface User {
  id: string;
  name: string;
  email: string;
}

export interface AuthResponse {
  user: User;
  accessToken: string;
  expiresAt: string;
}

export interface Category {
  id: string;
  name: string;
  type: TransactionType;
  color: string;
  isArchived: boolean;
}

export interface CreateCategoryRequest {
  name: string;
  type: TransactionType;
  color: string;
}

export interface FinancialTransaction {
  id: string;
  categoryId: string;
  categoryName: string;
  categoryColor: string;
  type: TransactionType;
  description: string;
  amount: number;
  occurredOn: string;
  notes?: string | null;
}

export interface CreateTransactionRequest {
  categoryId: string;
  type: TransactionType;
  description: string;
  amount: number;
  occurredOn: string;
  notes?: string | null;
}

export interface TransactionFilters {
  type?: TransactionType | '';
  categoryId?: string;
  from?: string;
  to?: string;
}

export interface DashboardFilters {
  year?: number;
  month?: number;
  type?: TransactionType | '';
  categoryId?: string;
}

export interface Dashboard {
  year: number;
  month: number;
  monthlyIncome: number;
  monthlyExpense: number;
  monthlyBalance: number;
  annualIncome: number;
  annualExpense: number;
  annualBalance: number;
  monthlyTotals: MonthlyTotal[];
  categoryTotals: CategoryTotal[];
  recentTransactions: FinancialTransaction[];
}

export interface MonthlyTotal {
  month: number;
  income: number;
  expense: number;
  balance: number;
}

export interface CategoryTotal {
  categoryId: string;
  categoryName: string;
  categoryColor: string;
  type: TransactionType;
  total: number;
}
