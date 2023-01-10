import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EmitterService } from 'src/app/services/emitter.service';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss'],
})
export class SidenavComponent implements OnInit {
  constructor(private router: Router, private emitter: EmitterService) {}

  ngOnInit() {}

  goTo(categoria: number) {
    this.router.navigate([`${categoria}`]);
    this.emitter.categoria.next(categoria);
  }
}
