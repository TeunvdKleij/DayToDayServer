# Introduction

Welcome to this project of DayToDay. This applications is made to get oversight in the tasks that need to be done in a day.  
This project started as a personal project, but now this project will be available to help others as well.
This is the server side of the application

# Installation

To use the application, you will need to copy the data from the .env.example into a .env file

1. Go to .env.example
2. Replace `[ownKey]` with a guid of your own
3. Make an .env file
4. Paste the data in the .env file
5. Run the application

For the moment, `localhost:3000` and `localhost:3001` are permitted as urls from the client, so make sure you will be able to use either port `3000` or `3001`.
If you want to use another port:
1. Go to `program.cs`
2. Look for the `builder.Services.AddCors`
3. Add a rule between the other `.WithOrigins` of `.WithOrigins([ownUrl])`

# Features

## Version 0.1

The first version are the use cases that represent the basic of usage of this application

- Have different groups to organise task per group (e.g. for seperation between home and work)
- Move tasks between days with an edit button
- Remove tasks when they are not needed anymore
- Navigate between the different days in multiple ways (Date, a today button and arrows to move one day at a time)
- Use a note at each day and group if you want to write out thoughts concerning the tasks or other cases

## Version 0.2
The added features:
- A way to toggle between seeing all the tasks from a group and just the tasks of the day

