-------------
Project Title
-------------

Online Library Website Project

---------------
Project Purpose
---------------

The motivation/purpose for writing this project was to prove and improve the coding skills of our internship development & testing team, as well as our ability to work together as a development/testing unit in an environment as close as possible to the real world. Our mentors played the role of the Client/Product Owner and Scrum Master, providing the User Stories with the functionalities that had to be implemented, as well as the Change Requests and Feedback, while our team played the role of a real Agile/Scrum development team, working in one-week-long Sprints during March-June, 2016.

-------------------
Project Description
-------------------

This project represents an Online Library website (which can be implemented either as an Intranet or Internet solution), offering several Library Management Information System features, such as: 

* Four system users:
  - Super Administrator, System Administrator, Librarian, User with different levels of authorization and privileges
* Signing in the system with a Google account and importing some basic account details from Google
* First sign in rule:
  - At first ever sign in into the system, the first user is able to access Roles page and assign himself the role of System Administrator
* Super Administrator role as a backup:
  - The legal owners/administrators of the website will be able to take full control of the website in case of emergencies (ex: site hijacking) or in case of a System Admin's inadequate behavior (he removes all other Admins or even removes himself as Admin)
  - Super Admin is able to sign in to the system using an automatically generated password stored locally in a specified folder on the server machine
  - Super Admin will have all authority and privileges as a regular SysAdmin, being able to restore other SysAdmins/Librarians into their roles
* User role management:
  - SysAdmins are able to add, remove, assign user roles
* Book management:
  - SysAdmins are able to add, remove, edit book attributes (such as: book title, author, ISBN, thematic category and subcategory, etc.), as well as to import basic book details from Amazon and Google Books stores
  - SysAdmins & Librarians are able to manage book loans: approve, reject, receive, declare books as lost, declare a given book's physical condition upon return
* Loan request functionality:
  - Signed in users are able to request a loan for a given book, based on their karma
  - Librarian is able to receive loan requests and either acccept or reject them
  - After loan is accepted, book is reserved by/assigned for the specific User for a given time period (ex: two weeks)
  - The user is then able to see when his book is due to be returned, by accessing his "My Loans" page
  - The user is also informed of his book return due date by email
* Search functionality:
  - Users are able to filter books using search criteria, such as: title, author(s), release date, ISBN, category, subcategory
  - Librarians are able to filter approved loans according to certain criteria (Book copy ID, ISBN)
* Loan history functionality:
  - Every loan request, loan approval, loan closing activities are logged into the database per each user and is available to be viewed by SysAdmins/Librarians.
* User Karma functionality:
  - All Users will earn a given karma, based on timely book return
  - If a User's karma is too low, he will not be able to request a loan for specific books with specific karma level requirements


------------------
Project Deployment
------------------

...

---------------
Project License
---------------

This project is licensed under the GNU General Public License, Version 3, as of 29 June 2007. Long story short, this license (1) asserts copyright on this software, and (2) gives you legal permission to copy, distribute and/or modify it. Also, it states that since this software is FREE, its developers/authors give you NO WARRANTY if you decide to use it for whatever purpose. Nobody obliges you to use this piece of software, therefore you are using it at your own risk. The developers/authors cannot be held responsible for any losses caused by the software or anybody using it, either because of certain bugs, or security flaws, or lack of functionalities, or malfunction of the software code. More details can be found in the GNU GPL ver.3 license.

