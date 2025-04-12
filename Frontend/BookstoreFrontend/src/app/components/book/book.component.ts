import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Book } from 'src/app/models/book.model';
import { BooksService } from 'src/app/services/books.service';

@Component({
  selector: 'app-books',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.css'],
})
export class BooksComponent implements OnInit {
  books: Book[] = [];

  constructor(
    private bookService: BooksService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.bookService.getAllBooks().subscribe({
      next: (books) => {
        this.books = books;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  deletebooks(id: number) {
    console.log(id);
    this.bookService.deleteBook(id).subscribe({
      next: (response) => {
        let currentUrl = this.router.url;
        this.router
          .navigateByUrl('/', { skipLocationChange: true })
          .then(() => {
            this.router.navigate([currentUrl]);
          });
      }
    });
  }
}
