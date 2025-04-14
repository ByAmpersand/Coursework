import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BookComponent } from './components/book/book.component';
import { AddBookComponent } from './components/add-book/add-book.component';
import { EditBookComponent } from './components/edit-book/edit-book.component';
import { BookDetailsComponent } from './components/book-details/book-details.component';


const routes: Routes = [
  {
    path: '',
    redirectTo: '/books', pathMatch: 'full'
  },
  {
    path: 'books',
    component: BookComponent
  },
  {
    path: 'books/add',
    component: AddBookComponent
  },
  {
    path: 'books/edit/:id',
    component: EditBookComponent
  },
  { path: 'book/:id', component: BookDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
