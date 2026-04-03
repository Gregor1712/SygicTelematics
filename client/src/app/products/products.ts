import {Component, inject, OnInit, ViewChild, AfterViewInit, OnDestroy} from '@angular/core';
import {CpuDTO, PaginationOfCpuDTO, ProductsClient, SwaggerException} from '../service/api-client';
import {MatTableDataSource} from '@angular/material/table';
import {MatTableModule} from '@angular/material/table';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {AutocompleteColumnComponent} from '../shared/components/autocomplete-column-component/autocomplete-column-component';
import {MatSort, MatSortModule} from '@angular/material/sort';
import {MatPaginator, MatPaginatorModule, PageEvent} from '@angular/material/paginator';
import {FilterParams} from '../shared/models/FilterParams';
import {finalize, of, Subscription, switchMap, takeUntil, tap} from 'rxjs';
import {CommonModule} from '@angular/common';
import {SafeUnsubscribeComponent} from '../shared/abstract/SafeUnsubscribeComponent';
import {MatDialog} from '@angular/material/dialog';
import {ErrorDialogComponent} from '../shared/components/error-dialog/error-dialog.component';
import {getErrorMessage} from '../shared/utils/error-message.util';
import {ApiError} from '../shared/models/apiError';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    AutocompleteColumnComponent,
    MatSortModule,
    MatPaginatorModule
  ],
  templateUrl: './products.html',
  styleUrls: ['./products.css'],
})
export class ProductsComponent extends SafeUnsubscribeComponent implements OnInit, AfterViewInit, OnDestroy {
  private shopService = inject(ProductsClient);
  private dialog = inject(MatDialog);
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  displayedColumns: string[] = ['name', 'socket', 'cores'];
  filterColumns: string[] = ['name-filter', 'socket-filter', 'cores-filter'];
  products?: PaginationOfCpuDTO;
  dataSource: MatTableDataSource<CpuDTO>;
  //totalCount = 0;
  filter: FilterParams = {};
  private _sortSub?: Subscription;

  nameOptions = ['AMD Ryzen 5 3600', 'AMD Ryzen 7 5800X', 'Intel Core i9-11900K'];
  socketOptions = ['LGA1700', 'AM4', 'AM5'];
  coresOptions = [8,12,16];
  private loading: boolean = false;

  constructor() {
    super();
    this.dataSource = new MatTableDataSource<CpuDTO>([]);
  }

  ngOnInit() {}

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.paginator.pageIndex = 0; // reset to first page on sort
    this.getProducts();
    this._sortSub = this.sort.sortChange.subscribe(() => {
      this.getProducts();
    });
  }

  onColumnFilter(column: keyof FilterParams, value: any) {
    const val = value?.toString().trim();
    if (!val) {
      this.filter[column] = undefined;
    } else {
      this.filter[column] = (column === 'cores') ? Number(val) : val;
    }
    this.getProducts();
  }

  getProducts() {
    const search = undefined;
    const pageIndex = (this.paginator?.pageIndex ?? 0) + 1;
    const pageSize = this.paginator?.pageSize ?? 10;

    const sort = (this.sort && this.sort.direction)
      ? `${this.sort.active}${this.sort.direction === 'desc' ? 'Desc' : 'Asc'}`
      : undefined;

    this.shopService.getProducts(this.filter?.name, this.filter?.socket, this.filter?.cores, sort, search, pageIndex, pageSize).subscribe({
      next: (response: PaginationOfCpuDTO) => {
        if (response?.data) {
          this.dataSource.data = response?.data ?? [];
          this.products = response;
        }
      },
      error: (err) => {
        let apiError: ApiError | undefined;
        if (SwaggerException.isSwaggerException(err) && err.response) {
          try {
            apiError = JSON.parse(err.response);
          } catch {}
        }

        const errorResult = getErrorMessage(apiError ?? err);
        this.dialog.open(ErrorDialogComponent, {
          width: '800px',
          data: {
            title: 'Unable to load products',
            message: errorResult.message,
            details: errorResult.details
          }
        });
      }
    });
  }

  onInsertPoplatok() {
    const search = undefined;
    const pageIndex = (this.paginator?.pageIndex ?? 0) + 1;
    const pageSize = this.paginator?.pageSize ?? 10;

    const sort = (this.sort && this.sort.direction)
      ? `${this.sort.active}${this.sort.direction === 'desc' ? 'Desc' : 'Asc'}`
      : undefined;

    this.loading = true;

    this.shopService.getProducts(
      this.filter?.name,
      this.filter?.socket,
      this.filter?.cores,
      sort,
      search,
      pageIndex,
      pageSize
    ).pipe(
      // Use tap for side effects (assigning data)
      tap((result: PaginationOfCpuDTO) => {
        if (result?.data) {
          this.dataSource.data = result.data;
          this.products = result;
        }
      }),
      // Use finalize to ensure loading is turned off even on error
      finalize(() => this.loading = false),
      // Unsubscribe is important to prevent memory leaks!
      takeUntil(this.unsubscribe$)
    ).subscribe({
      error: (err) => {
        // Handle your error here
        console.error(err);
        // SwalUtils.createOrAddToQueueDefault('Error', 'Message', 'error');
      }
    });
  }

  // onInsertPoplatok() {
  //
  //   const search = undefined;
  //   // paginator.pageIndex is 0-based; backend expects 1-based page index -> add 1
  //   const pageIndex = (this.paginator?.pageIndex ?? 0) + 1;
  //   const pageSize = this.paginator?.pageSize ?? 10;
  //
  //   const sort = (this.sort && this.sort.direction)
  //     ? `${this.sort.active}${this.sort.direction === 'desc' ? 'Desc' : 'Asc'}`
  //     : undefined;
  //
  //   this.loading = true;
  //   //poplatok.idPacienta = this.idPacienta;
  //
  //   this.shopService.getProducts(this.filter?.name, this.filter?.socket, this.filter?.cores, sort, search, pageIndex, pageSize).pipe(
  //     switchMap((result: PaginationOfCpuDTO) => {
  //       if (result?.data) {
  //         this.dataSource.data = result?.data ?? [];
  //         this.products = result;
  //       } else {
  //         //SwalUtils.createOrAddToQueueDefault('Poplatok', 'Nepodarilo sa vložiť Poplatok.', 'error');
  //         //return of(null);
  //       }
  //     }),
  //     finalize(() => this.loading = false),
  //     //takeUntil(this.unsubscribe$)
  //   ).subscribe();
  // }




  override ngOnDestroy(): void {
    this._sortSub?.unsubscribe();
  }

  protected pageChange(event: PageEvent) {
    this.getProducts();
  }

}
