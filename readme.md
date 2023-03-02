# Questions/Assumptions

## Index

It says any available Stock but then it says to show in red if less than 0

Maybe a column to show Reserved Quantity should be added

Should the “More Info” link be placed at the end of the row?

On click of Action if an error occurred what will be the action? I have assumed still open Product page with Error showing.

## Warehouse API

If unable to find Product then should an error be returned? At the moment I have assumed to return an Empty Product.

It says to check for negative numbers and return an error – shouldn’t it be checking for 0 amount and returning an error

The “UpdateQuantities” function and the “Insert” should return an object saying whether it was successful. At the moment I am just assuming it was successful (even though I can manually check that it was) and returning a success of true.

## Warehouse API - Order

Says “Action: GET”

But we are giving an UpdateQuanityRequest so needs to be “PUT” 

## Warehouse API – Add

Unique FileName – how many checks of the name followed by a unique id will there be? I have assumed <10 but this could potentially add duplicates. Would need to confirm what number to check up to and what happens when reach this number? Should it return an error with too many duplicate names being added.


# Improvements if More Time

Further Styling

Add more Error Capturing

More Comments

More Tests – have only added a sample which is covering 25.6% Line Coverage and 15.2% Branch Coverage

Add logger

