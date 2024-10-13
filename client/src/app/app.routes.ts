import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';

export const routes: Routes = [
    { path: "", component: HomeComponent },
    { path: "error", component: TestErrorComponent },
    { path: "server-error", component: ServerErrorComponent },
    { path: "not-found", component: NotFoundComponent },
    {
        path: "",
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [{ path: "members", component: MemberListComponent },
        { path: "members/:username", component: MemberDetailComponent },
        { path: "lists", component: ListsComponent },
        { path: "messages", component: MessagesComponent }]
    },
    { path: "**", component: HomeComponent, pathMatch: 'full' }
];
