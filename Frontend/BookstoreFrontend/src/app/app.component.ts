import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  isMenuOpen: boolean = false;
  
  searchQuery: string = '';

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }

  searchBooks(): void {
    console.log('Searching for books with title:', this.searchQuery);
    // Додайте логіку для пошуку книг за назвою
  }
}