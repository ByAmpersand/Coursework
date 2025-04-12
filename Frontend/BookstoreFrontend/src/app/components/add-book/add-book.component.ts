import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Book } from 'src/app/models/book.model';
import { BooksService } from 'src/app/services/books.service';

@Component({
  selector: 'app-add-book',
  templateUrl: './add-book.component.html',
  styleUrls: ['./add-book.component.css'],
})
export class AddBookComponent {
  newBook: Book = {
    id: 0,
    name: '',
    genre: '',
    price: 0,
    language: '',
    numberOfPages: 0,
    publicationYear: 0
  };
 
  constructor(
    private bookService: BooksService,
    private router: Router
  ) {}

  addBook() {
    this.bookService.addBook(this.newBook).subscribe({
      next: (book) => {
        this.router.navigate(['books']);
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
}
