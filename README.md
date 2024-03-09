# Course Management System - README

## Overview
This project encompasses two main components: UserManager, a Web API serving as an authentication service, and SchoolLab, another Web API managing laboratory and resource reservations. The authentication process occurs through UserManager, where users log in via a Login Controller, communicating with UserManager's API. Once authenticated, users can perform actions within the SchoolLab API.

### UserManager
- **Authentication Flow:**
  - Users authenticate through the SchoolLab Login Controller, which forwards requests to the UserManager API.
  - UserManager confirms or denies authentication for subsequent actions within SchoolLab.

- **User Management (Super Admin):**
  - UserManager contains a controller allowing CRUD operations for super admins.
  - Super admins can retrieve the list of users, details of a single user, modify user information, delete users, and create new users.

### SchoolLab
- **Authentication and Authorization:**
  - The SchoolLab Login Controller communicates with UserManager for user authentication.
  - LabAdminController allows only lab admin users to perform CRUD operations within the laboratory.

- **Lab Administration:**
  - LabAdminController handles CRUD operations for laboratories, computers, and software resources.
  - Admins can retrieve lists, create, modify, and delete computers.

- **User Operations:**
  - LabUserController enables CRUD operations for user reservations.
  - Users can retrieve all reservations, create new reservations, and delete existing reservations.

### FileManager Library
In addition to the UserManager and SchoolLab components, the project includes an external FileManager library. Both UserManager and SchoolLab utilize this library for data persistence. It's important to note that while the FileManager library provides the service, each web API (UserManager and SchoolLab) independently manages data storage within its own directories.
___
# UserManager API Endpoints

## Authentication Operations (AuthenticationController)

### Get Authentication Challenge
- **Endpoint:** `GET /api/authentication/{username}`
- **Description:** Obtain an authentication challenge for a given username.

### Perform Authentication
- **Endpoint:** `GET /api/authentication/{username}/{challenge}`
- **Description:** Perform user authentication using the provided challenge. Returns user details if successful.

### Check Authentication
- **Endpoint:** `GET /api/authentication/check-auth/{token}`
- **Description:** Verify the validity of a user's token and return the user's role.

## User Administration (AdminManagerController)

### Get List of Users
- **Endpoint:** `GET /api/user-manager/admin/users`
- **Description:** Retrieve the list of users. (Requires SuperAdmin role)

### Get User Details
- **Endpoint:** `GET /api/user-manager/admin/users/{id}`
- **Description:** Retrieve details of a specific user. (Requires SuperAdmin role)

### Create User
- **Endpoint:** `POST /api/user-manager/admin/users/create`
- **Description:** Create a new user. (Requires SuperAdmin role)

### Update User
- **Endpoint:** `PATCH /api/user-manager/admin/users/update/{id}`
- **Description:** Update user details. (Requires SuperAdmin role)

### Delete User
- **Endpoint:** `DELETE /api/user-manager/admin/users/delete/{id}`
- **Description:** Delete a user. (Requires SuperAdmin role)

## User Operations (UsersController)

### Get User Details
- **Endpoint:** `GET /api/users/{id}`
- **Description:** Retrieve details of the authenticated user.

### Update User Details
- **Endpoint:** `PATCH /api/users/update-user/{id}`
- **Description:** Update user details for the authenticated user.
___
# SchoolLab API Endpoints

## Login Operations (LoginController)

### Get Authentication Challenge
- **Endpoint:** `GET /api/login/{username}`
- **Description:** Obtain an authentication challenge for a given username.

### Perform Authentication
- **Endpoint:** `GET /api/login/{username}/{challenge}`
- **Description:** Perform user authentication using the provided challenge. Returns user details if successful.

## Lab Administration (LabAdminController)

### Get List of Labs
- **Endpoint:** `GET /api/admin/labs`
- **Description:** Retrieve the list of laboratories.

### Get Lab Details
- **Endpoint:** `GET /api/admin/labs/{labId}`
- **Description:** Retrieve details of a specific laboratory.

### Get List of Computers in a Lab
- **Endpoint:** `GET /api/admin/labs/{labId}/computers`
- **Description:** Retrieve the list of computers in a specific laboratory.

### Create Computer in a Lab
- **Endpoint:** `POST /api/admin/labs/{labId}/computers/create`
- **Description:** Create a new computer in a specific laboratory.

### Update Computer in a Lab
- **Endpoint:** `PATCH /api/admin/labs/{labId}/computers/update/{computerId}`
- **Description:** Update information for a specific computer in a laboratory.

### Delete Computer from a Lab
- **Endpoint:** `DELETE /api/admin/labs/{labId}/computers/delete/{computerId}`
- **Description:** Delete a computer from a specific laboratory.

## User Operations (LabUserController)

### Get List of Labs for a User
- **Endpoint:** `GET /api/user/labs`
- **Description:** Retrieve the list of laboratories for the authenticated user.

### Get Reservations for a User
- **Endpoint:** `GET /api/user/labs/reservation/{userId}`
- **Description:** Retrieve reservations for a specific user.

### Create Reservation
- **Endpoint:** `POST /api/user/labs/reservation/create`
- **Description:** Create a new reservation for the authenticated user.

### Delete Reservation
- **Endpoint:** `DELETE /api/user/labs/reservation/delete/{reservationId}`
- **Description:** Delete a reservation for the authenticated user.
___
# Data Transfer Objects (DTOs):

- **UserDTO:**
  - Facilitates data transfer between UserManager and SchoolLab and frontend.
  
```csharp
public class UserDto
{
    public string Id { get; }
    public string Username { get; }
    public string Name { get; }
    public string Surname { get; }
    public string Token { get; }
    public UserRole UserRole { get; }
    public int HoursOfUse { get; }
    public int MaxHoursBookingAllowed { get; }

    public UserDto(string id, string username, string name, string surname, string token, UserRole userRole = UserRole.SchoolUser, int hoursOfUse = 0, int maxHoursBookingAllowed = 2)
    {
        Id = id;
        Username = username;
        Name = name;
        Surname = surname;
        Token = token;
        UserRole = userRole;
        HoursOfUse = hoursOfUse;
        MaxHoursBookingAllowed = maxHoursBookingAllowed;
    }    
}
```

- **ComputerDTO, ReservationDTO:**
  - Enables data transfer from SchoolLab to the frontend for computers and reservations.

```csharp
public class ComputerDto
{
    public string Description { get; }
    public ComputerStatus Status { get; }
    public string LabAssigned { get; }

    public ComputerDto(string description, ComputerStatus status, string labAssigned)
    {
        Description = description;
        Status = status;
        LabAssigned = labAssigned;
    }
}

public class ReservationDto
{
    public string LabId { get; }
    public DateTime Date { get; }
    public int TimeSlot { get; }
    public string UserId { get; }

    public ReservationDto(string labId, DateTime date, int timeSlot, string userId)
    {
        LabId = labId;
        Date = date;
        TimeSlot = timeSlot;
        UserId = userId;
    }
}
```