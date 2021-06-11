Feature: Pay crypto to access pdf report feature
	Allows 2 more roles to access the pdf report feature by paying with crypto 

@edgyJackson
Scenario Outline: SpillTracker can generate a PDF of spill report for Employees 
	Given username is 'UserName'
	  And is on the form results page
	  And user is Employee role 'Employee' 
	 When the user activates the generate pdf report button
	  And the user is directed to the coin payment page
	  And the User makes a coin payment 'CoinPayment'
	 Then they are directed to the generated pdf report page 'answer'
	 And They can download the pdf report.
	 Examples:
      | UserName   | CoinPayment | answer |
      | TaliaK     | True        | True   |
      | ZaydenC    | True        | True   |
      | DavilaH    | True        | True   |
      | KrzysztofP | False       | False  |
      | God        | False       | False  |

