import { Injectable } from "@angular/core";
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberListResolver implements Resolve<User[]>{
  
    constructor(private userService:UserService, private router:Router) {}
   resolve(route:ActivatedRouteSnapshot):Observable<User[]>{
       return this.userService.getUsers().pipe(
           catchError(error=>{
               console.log(error);
               this.router.navigate(['/home']);
               return of(null);
           })
       );
   }
}