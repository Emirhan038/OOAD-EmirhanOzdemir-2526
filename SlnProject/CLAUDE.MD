# Project conventions for the AI agent — Dokterspraktijk (college OOAD assignment)


## Context
- This is a graded individual college assignment (course: Object Oriented Analysis & Design).
- The grader ONLY accepts techniques taught in our C# course. Using a non-allowed technique
  scores 0 on that whole section. Treat the "NEVER USE" list below as absolute.
- All user-facing UI text must be in **Dutch**. Code identifiers/comments may be English.
- Target framework: **.NET 10**, WPF, C#. **Exactly one** NuGet package is allowed and
  required: `Microsoft.Data.SqlClient` (the official Microsoft SQL Server provider — it is
  NOT part of the base .NET framework, and connecting to SQL Server in .NET 10 is
  impossible without it; talking to a SQL database is a required technique of this
  assignment). No other NuGet packages — no EF, no Dapper, no Newtonsoft, nothing else.


## Architecture (mandatory)
- One solution, three projects:
  1. WPF app for doctors
  2. WPF app for patients
  3. Class Library (the business layer)
- ALL database (SQL) code lives in the Class Library. The two WPF apps must contain
  ZERO SQL. They only call public properties/methods on library classes.
- CRUD lives **inside the entity classes themselves** (e.g. a `Patient` knows how to
  save/update/delete itself). No separate DataLayer / DataContext / Repository class.
- `Patient` and `Dokter` share most fields → use **inheritance**: a base class
  `Persoon` holds the shared fields/behaviour; `Patient` and `Dokter` extend it.
- Notification preference is an **enum** with four values: Geen, Mail, Sms, Beide
  (mapped to int 0,1,2,3 in the DB).
- Exception handling and try/catch go in the WPF code-behind (application layer),
  NOT in the Class Library. The library may let exceptions bubble up.
- Errors are shown to the user in a `TextBlock` in the window — **never a MessageBox**.


## Allowed techniques (course-taught)
- Plain C#: variables, if/else, switch, for/while/foreach, methods, classes, objects,
  static members, enums, inheritance.
- Collections: arrays and `List<T>` (loop over them with `foreach`/`for`).
- ADO.NET database access via **`Microsoft.Data.SqlClient`** (`using Microsoft.Data.SqlClient;`):
  `SqlConnection`, `SqlCommand` with **named parameters** (`@id`), `SqlDataReader`,
  `ExecuteReader`, `ExecuteNonQuery`, `ExecuteScalar`. Map rows to objects manually in a
  loop. Open the connection in a `try`, close it in a `finally` (or a `using` block).
- File handling: `System.IO.File`, `MemoryStream`, `BitmapImage`.
- WPF: building controls in XAML, and building controls programmatically in code-behind
  (needed for the dynamic card grid). `Frame` + `Page` navigation. Standard controls:
  `Button`, `TextBox`, `PasswordBox`, `Calendar`, `ComboBox`, `ListBox`,
  `RadioButton`, `Image`, `TextBlock`, `WrapPanel`, `Grid`, `StackPanel`.
- SHA-256 hashing via `System.Security.Cryptography`.


## NEVER USE (each = automatic 0 on that part — no exceptions)
- `var` (always write the explicit type, including in `foreach`).
- LINQ of any kind (`.Where`, `.Select`, `.FirstOrDefault`, `.Any`, query syntax…).
  Use `foreach` loops instead.
- Data binding / `{Binding}` / `INotifyPropertyChanged` / `ObservableCollection`.
  Set every control's value from code-behind explicitly.
- `DataGrid`, `GridView`, `ListView`. (Use `ListBox`, or build a `WrapPanel`/`Grid`
  of cards in code-behind.)
- `async` / `await` (all DB and file calls are synchronous).
- Tuples, `dynamic`, `var`, expando objects, `struct`, type-switch / pattern-matching
  switch, `case ... when` guards, `out` / `ref` parameters, `UserControl`,
  `Dispatcher.Invoke`/`BeginInvoke`.
- `int.TryParse` / `DateTime.TryParse` are FORBIDDEN because they use `out`.
  Validate input with `try { int x = int.Parse(s); } catch { ... }` (or `Convert.ToInt32`).


## Code style
- Comment richly: every method gets a short purpose comment; non-obvious lines get a
  why-comment. The grader expects to read the code and understand the reasoning.
- One feature at a time. After each feature: it must build and run with no crash.
- If you borrow a snippet for a small sub-problem, add a comment `// source: <url>`.
- Pre-fill the login fields with a valid test account to make grading/testing easy.
- Show a placeholder image when a profile photo is missing.


## Working mode
- When asked for a plan, produce a step-by-step plan and STOP (plan mode) — do not edit.
- Otherwise implement ONE requested feature, then summarise what changed and stop so I
  can review and commit before the next feature.