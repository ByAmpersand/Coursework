import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.css']
})
export class BookComponent {
  books = [
    {
      id: 1,
      name: 'Book Title 1',
      genre: 'Fiction',
      price: 19.99,
      language: 'English',
      numberOfPages: 320,
      publicationYear: 2021,
      imageUrl: 'assets/book1.jpg'
    },
    {
      id: 2,
      name: 'Book Title 2',
      genre: 'Non-Fiction',
      price: 24.99,
      language: 'English',
      numberOfPages: 250,
      publicationYear: 2020,
      imageUrl: 'assets/book2.jpg'
    },
    // Додайте більше книг
  ];

  constructor(private router: Router) {}

  viewBookDetails(bookId: number): void {
    this.router.navigate(['/book', bookId]);
  }
}