# Frontend Rules

Rules verified against the current codebase. These supplement `map.md` (structure) with behavioral rules.

---

## Member Visibility Convention

Template-bound members are `protected`. Internal-only members are `private readonly`.

```typescript
// Injected services — never referenced in templates
private readonly usersApi = inject(UsersApi);
private readonly toast = inject(ToastService);

// State and methods used in templates
protected readonly loading = signal<boolean>(true);
protected readonly users = signal<UserDto[]>([]);
protected onSubmit(): void { ... }
```

---

## Dependency Injection Style

- **Components** use the `inject()` function.
- **API client services** use constructor injection.

```typescript
// Component
export class UsersPageComponent {
  private readonly usersApi = inject(UsersApi);
}

// API client service
export class UsersApi {
  constructor(private readonly api: ApiClient) {}
}
```

---

## Template Control Flow

Use Angular 17+ built-in control flow. Do not use structural directives (`*ngIf`, `*ngFor`).

- `@if (condition()) { ... } @else { ... }`
- `@for (item of items(); track item.id) { ... }`

`@for` always requires a `track` expression.

---

## Submitting Signal Pattern

Every modal that makes a write request uses a `submitting` signal to prevent double-submit:

```typescript
protected readonly submitting = signal(false);

protected onSubmit(): void {
  if (this.submitting()) return;       // block double-submit

  this.submitting.set(true);
  this.api.create(dto).subscribe({
    next: () => {
      this.toast.success('Created');
      this.created.emit();
      this.closed.emit();              // modal closes — no reset needed
    },
    error: (err: unknown) => {
      this.toast.errorMessage(err);
      this.submitting.set(false);      // re-enable on error only
    }
  });
}
```

In the template, disable the submit button:

```html
<button type="submit" [disabled]="submitting()">Create</button>
```

---

## Local Display-Row Interface

Pages define a local `interface` to transform raw DTOs into a shape suited for the template. This keeps template logic minimal and separates API shape from display shape.

```typescript
interface UserRow {
  id: number;
  name: string;
  email: string;
  status: 'Active' | 'Inactive';
}

protected readonly userRows = computed(() =>
  this.users().map((u) => ({
    id: u.id,
    name: this.displayName(u),
    email: u.userName,
    status: u.inactivatedDateTime ? 'Inactive' as const : 'Active' as const
  }))
);
```

---

## Host Class on Shell-Less Pages

Pages that render outside `AppShellLayoutComponent` (e.g., `/login`) must own their full-page layout via the `host` metadata property:

```typescript
@Component({
  host: {
    class: 'block min-h-screen bg-[var(--bg)] text-[var(--fg)]'
  }
})
```

Without this, the page won't fill the viewport.

---

## Return URL Pattern

The `authGuard` captures the attempted URL and passes it as a query parameter to `/login`:

```typescript
const attemptedUrl = `/${segments.map((s) => s.path).join('/')}`;
return router.createUrlTree(['/login'], {
  queryParams: { returnUrl: attemptedUrl }
});
```

On successful login, `AuthService.login()` navigates to `returnUrl` if it starts with `/`, otherwise falls back to `/dashboard`.

---

## Component Selector Prefix

Every component selector uses the `app-` prefix:

- `app-users-page`
- `app-add-user-modal`
- `app-shell-layout`
- `app-sidebar`

---

## API Caching

### How It Works

GET requests go through `ApiClient.getCached()`, which applies a **5-minute TTL** and in-flight deduplication. If a valid cached response exists, no HTTP call is made. If the same request is already in flight, the same Observable is returned instead of making a duplicate request.

Write methods (`post`, `patch`) bypass the cache entirely — they always make a real HTTP call.

### Cache Invalidation Rule

After any write that changes data a cached GET would return, the API client method must clear the relevant cache keys using `clearGetCache()` inside a `tap()`:

```typescript
create(dto: CreateUserRequestDto): Observable<CreateUserResponseDto> {
  return this.api.post<CreateUserResponseDto>('/users', dto).pipe(
    tap(() => {
      this.api.clearGetCache('/users');       // clears active list
      this.api.clearGetCache('/users/all');   // clears all list
    })
  );
}
```

### Cross-Entity Cache Clearing

Some writes affect cached data for a **different** entity. In those cases, clear the other entity's cache path:

```typescript
// StockLotAdjustmentsApi — creating an adjustment changes StockLot.AmountOfBars
create(dto): Observable<{ id: number }> {
  return this.api.post(_PATH, dto).pipe(
    tap(() => {
      this.api.clearGetCache(STOCK_LOTS_PATH);  // clear stock-lots, not adjustments
    })
  );
}
```

The same applies to `ShippingReceivingApi.receive()` — it clears the stock-lots cache because the workflow creates a new stock lot.

### What to Clear

Clear every GET path that would now return stale data:

| Write operation | Cache keys to clear |
|----------------|---------------------|
| Create entity | `/{entity}` and `/{entity}/all` |
| Update entity | `/{entity}/{id}` (if cached), `/{entity}`, `/{entity}/all` |
| Inactivate entity | `/{entity}` and `/{entity}/all` |
| Write that affects a related entity | That related entity's list path |

`clearGetCache()` performs an exact key match — clearing `/users` does **not** clear `/users/all` or `/users/5/roles`. Each path must be cleared individually.

---

## Required Inputs on Modals

Modals that receive data from a parent page use `@Input({ required: true })` for mandatory values:

```typescript
@Input({ required: true }) userId!: number;
@Input({ required: true }) fullName!: string;
@Input({ required: true }) email!: string;
```

The `!` (definite assignment) is needed because Angular sets the value after construction. `required: true` enforces that the parent must provide the binding.
