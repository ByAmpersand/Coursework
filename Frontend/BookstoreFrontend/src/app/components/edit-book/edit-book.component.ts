import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from 'src/app/models/book.model';
import { BooksService } from 'src/app/services/books.service';

@Component({
  selector: 'app-edit-Book',
  templateUrl: './edit-Book.component.html',
  styleUrls: ['./edit-Book.component.css'],
})
export class EditBookComponent implements OnInit {
  updateBookRequest: Book = {
    id: 0,
    name: '',
    genre: '',
    price: 0,
    language: '',
    numberOfPages: 0,
    publicationYear: 0
  };
  constructor(
    private BookService: BooksService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id');

        if (id) {
          this.BookService.getBook(Number(id)).subscribe({
            next: (Book) => {
              this.updateBookRequest = Book;
            },
          });
        }
      },
    });
  }
  updateBook() {
    this.BookService
      .updateBook(this.updateBookRequest.id, this.updateBookRequest)
      .subscribe({
        next: (response) => {
          this.router.navigate(['books']);
        },
        error: (error) => {
          console.log(error);
        },
      });
  }
}
