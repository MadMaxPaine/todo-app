import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, debounceTime, takeUntil } from 'rxjs';
import {
  TasksService,
  Task,
  Category,
} from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { HostListener } from '@angular/core';
@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks.component.html',
  
})
export class TasksComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private search$ = new Subject<string>();

  tasks: Task[] = [];
  filteredTasks: Task[] = [];
  categories: Category[] = [];

  title = '';
  description = '';
  categoryId: number | null = null;

  selectedCategory: number | null = null;

  search = '';

  page = 1;
  pageSize = 10;
  totalCount = 0;
  isCreateOpen = false;
  isEditOpen = false;
  editingTask: Task | null = null;
  
  @HostListener('document:keydown.escape')
  onEscPress() {
    if (this.isCreateOpen) {
      this.closeCreate();
    }

    if (this.isEditOpen) {
      this.closeEdit();
    }
  }
  constructor(
    private tasksService: TasksService,
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadCategories();

    this.tasksService.tasks$
      .pipe(takeUntil(this.destroy$))
      .subscribe((tasks) => {
        this.tasks = tasks;
        this.applyFilter();
      });

    this.search$
      .pipe(debounceTime(300), takeUntil(this.destroy$))
      .subscribe((value) => {
        this.search = value;
        this.page = 1;
        this.loadTasks();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTasks() {
    this.tasksService
      .getTasks(this.page, this.pageSize, this.selectedCategory, this.search)
      .subscribe((res) => {
        this.totalCount = res.totalCount;
      });
  }

  loadCategories() {
    this.tasksService
      .getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe((res) => (this.categories = res));
  }

  onSearch(value: string) {
    this.search$.next(value);
  }

  setFilter(categoryId: number | null) {
    this.selectedCategory = categoryId;
    this.page = 1;
    this.loadTasks();
  }

  nextPage() {
    this.page++;
    this.loadTasks();
  }

  prevPage() {
    if (this.page === 1) return;
    this.page--;
    this.loadTasks();
  }

  applyFilter() {
    this.filteredTasks =
      this.selectedCategory === null
        ? this.tasks
        : this.tasks.filter((t) => t.categoryId === this.selectedCategory);
  }

  createTask() {
    if (!this.title || this.categoryId === null) return;

    this.tasksService
      .createTask({
        title: this.title,
        description: this.description,
        categoryId: this.categoryId,
      })
      .subscribe(() => {
        this.title = '';
        this.description = '';
        this.categoryId = null;
      });
  }

  toggle(task: Task) {
    this.tasksService.toggleTask(task.id, !task.isCompleted).subscribe();
  }

  deleteTask(id: number) {
    this.tasksService.deleteTask(id).subscribe();
  }

  startEdit(task: Task) {
    this.editingTask = { ...task };
  }

  saveEdit() {
  if (!this.editingTask) return;

  this.tasksService
    .updateTask(this.editingTask.id, {
      title: this.editingTask.title,
      description: this.editingTask.description,
      categoryId: this.editingTask.categoryId ?? null,
    })
    .subscribe((updatedTask) => {
      const index = this.tasks.findIndex(t => t.id === updatedTask.id);

      if (index !== -1) {
        this.tasks[index] = updatedTask;
      }

      this.applyFilter();
      this.closeEdit();
    });
}

  cancelEdit() {
    this.editingTask = null;
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  openCreate() {
    this.isCreateOpen = true;
  }

  closeCreate() {
    this.isCreateOpen = false;
  }

  openEdit(task: Task) {
    this.editingTask = { ...task };
    this.isEditOpen = true;
  }

  closeEdit() {
    this.isEditOpen = false;
    this.editingTask = null;
  }
}
