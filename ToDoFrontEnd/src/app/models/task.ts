export interface Task {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
  createdAt: string;
  categoryId?: number;
  categoryName?: string;
}

export interface CreateTask {
  title: string;
  description: string;
  categoryId?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}