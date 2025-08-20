# Read this first!
Hi, a quick outline of my approach regarding the tasks

## Before I started: A few things broken that I changed
- Migration did not work on startup. I fixed this by creating a dummy migration.
- MongoDB did not like the attribute **[BsonDateTimeOptions(DateOnly = true)]** . So I remved these and changed the 
underlying type form DateTime to DateOnly.

# Programming Task
In the order that I started them, not the order that they were presented.

## Task 4
I started this one first, as a way of ensuring that:
- On completion of Task 1 I would have an architecture that had good separation of business logic from data handling
- Ensure that I was (mostly) respecting the Single Responsibility principle.
- That I could write tests that were not too complex or brittle, and rely only on what the expected functionality of 
each class was, not how it was implemented.

To that end:
- I started with a couple of Integration to check some of the REST API end-points (thanks for providing one already).
- I added Moq to the Claims.Tests project so that I could inject requests and responses for my Unit Tests.
- I used a TDD approach for tasks 1, 3 and 5.
- I tried to keep the use of seams to a minimum, though this proved necessary for testing some of the Date-handling logic

Tests are broken into 3 categories:
- Integration
- Unit
- Failing (See Task 5: BadPremiumCalculator)

## Task 1
I went for a traditional layered approach here, namely
- API Layer (very light): No validation.
- Service Layer (Business Logic)
- Repository Layer: To enforce a contract between the Service and Data Layers
- Data Layer: ORMs. Made the Tables public and moved complex up to Service Layer
- Persistance Layer: Did not change either of the DBs

All classes were hooked up via DI in program.cs 

Tests to ensure that this worked:
- ClaimsServiceTests
- CoverServicesTests
- ClaimsControllerTests (Integration)
- CoversControllerTests (Integration)

### Task 2
Settled on a very simple approach for this due to time-constraints.
- Create generic IValidator<T> interface
- Implement separately for Claims and Covers

Tests to ensure that this worked:
- ClaimValidatorTests
- CoverValidatorTests

A few shortcomings:
Each of these is a simple Validate function with a few if statements. 
This might not scale and would not make a good solution if serving multiple B2B customers.
Alternatives might include either:
- Dependency Injection: Custom Rules DLL per customer
- Rules Engine approach

## Task 5
- Created an interface IPremiumCalculator
- Wrote some tests (PremiumCalculatorTests)
- Copy/Pasted original code into BadPremiumCalculator (Currently passing 4/33 tests)
- Implemented a working solution: TranchedPremiumCalculator

A few notes on the implementation:
- TranchedPremiumCalculator.BasePremium + TranchedPremiumCalculator.GetTranches should both 
ideally come from data. I've hard-coded them due to time constraints. 
- This suffers from the same shortcomings as my solution to Task 3, namely that Premium calculation is likely very 
complicated and might vary from customer to customer.

## Task 3
One crucial assumption here: that "Fire and Forget" is an acceptable solution to logging audits.
Otherwise, a synchronous, blocking solution might be preferable.

I used Channels to simulate a Queue and placed them in a 2nd IAuditor implementation that calls 
the original Auditor class. The implementation is for Demo purposes and is not robust.

A better solution would be to use an actual service bus with Topic-> Queue -> DLQ 

If "Fire and Forget" is not acceptable, but the operation still needs to be non-blocking 
(i.e. if it takes several seconds), then an asynchronous approach using web-sockets or returning 
a token and using long-polling would probably offer a good solution.