import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, tap } from 'rxjs';

export interface Task {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
  createdAt: string;
  categoryId?: number;
  categoryName?: string | null;
}

export interface Category {
  id: number;
  name: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class TasksService {
  private baseUrl = 'http://localhost:5261/api';

  private tasksSubject = new BehaviorSubject<Task[]>([]);
  tasks$ = this.tasksSubject.asObservable();

  constructor(private http: HttpClient) {}

  getTasks(
    page = 1,
    pageSize = 10,
    categoryId?: number | null,
    search?: string
  ) {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (categoryId != null) {
      params = params.set('categoryId', categoryId);
    }

    if (search && search.trim().length > 0) {
      params = params.set('search', search);
    }

    return this.http
      .get<PagedResult<Task>>(`${this.baseUrl}/tasks`, { params })
      .pipe(
        tap(res => {
          this.tasksSubject.next(res.items);
        })
      );
  }

  createTask(dto: {
    title: string;
    description: string;
    categoryId: number | null;
  }) {
    return this.http.post<Task>(`${this.baseUrl}/tasks`, dto).pipe(
      tap(task => {
        this.tasksSubject.next([task, ...this.tasksSubject.value]);
      })
    );
  }

  updateTask(id: number, dto: any) {
    return this.http.put<Task>(`${this.baseUrl}/tasks/${id}`, dto).pipe(
      tap(updated => {
        this.tasksSubject.next(
          this.tasksSubject.value.map(t =>
            t.id === id ? updated : t
          )
        );
      })
    );
  }

  toggleTask(id: number, isCompleted: boolean) {
    return this.http.patch<Task>(`${this.baseUrl}/tasks/${id}`, {
      isCompleted
    }).pipe(
      tap(updated => {
        this.tasksSubject.next(
          this.tasksSubject.value.map(t =>
            t.id === id ? updated : t
          )
        );
      })
    );
  }

  deleteTask(id: number) {
    return this.http.delete(`${this.baseUrl}/tasks/${id}`).pipe(
      tap(() => {
        this.tasksSubject.next(
          this.tasksSubject.value.filter(t => t.id !== id)
        );
      })
    );
  }

  getCategories() {
    return this.http.get<Category[]>(`${this.baseUrl}/categories`);
  }
}