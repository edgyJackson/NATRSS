@PDFReportingService
Feature: Generating PDF Report Of Spill
![PDF Service](https://specflow.org/wp-content/uploads/2020/09/calculator.png)
Simple PDF Service for generating **PDF Spill Report Documentation** Files

Link to a feature: [PDFReportService.feature](SpillTrackerBDDTests/Features/PDFReportService.feature)


Background:
	Given the following users exist
	  | UserName   | Email                 | FirstName | LastName | Password  | Role            | 
	  | TaliaK     | admin@example.com     | Talia     | Knott    | Admin123! | Admin           |
	  | ZaydenC    | clark@example.com     | Zayden    | Clark    | Hello123# | Employee        |
	  | DavilaH    | hareem@example.com    | Hareem    | Davila   | Hello123# | FacilityManager |
	  | KrzysztofP | krzysztof@example.com | Krzysztof | Ponce    | Hello123# |                 |
    
	
	
	
Scenario: SpillTracker can generate a PDF of spill report for admins
	Given username is TaliaK
	  And user is logged In 
	  And users role is Admin 
	  And user navigates to Form/Details/1 page
	 When the user activates the generate pdf report button
	 Then the pdf is displayed
	 





