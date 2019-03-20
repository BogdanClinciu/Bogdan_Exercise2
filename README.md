# Bogdan_Exercise2

Requirement:

Implement a system to process single variable integer coefficients.
Have the system perform the following operations on the polynomials:
· addition, substraction
· multiplication
· derivation
· integration on an undefined domain
· draw the graph of the input polynomials.


Implementation:

The chosen type for the polynomial type is a Dictionary<int, float> with the key as exponent and value as coeficient.
  This datatype was chosen becasue of the unique value of the keys in a dictionary. A polynomial should have only one of each of its exponent values. 

PolynomialMath namspace is used to perform the mathemathical operations on the polynomial type.
  Adition, substraction and multiplication are done by simple for loops that perform the operations on compatible members of a polynomial.
  Derivation and integration works in the same way going trough the elements of the dictionary-polynomial and apply the corect operation.
  Division TBD.
  
PolynomialUtils namspace is used to validate user input and to parse polynomials in mathemathical notation.
  For input a custom input validator was needed; In this case, it checks the input character against an array of accepted characters and uses special cases for punctuation and spaces in order to neatly separate the elements of the polynomial.
  Parsing the polynomal is a matter of a for loop where for each element we assign the propper sign, coeficient, x character, and exponent.

Two polynomial ui groups are used to input, view and operate on the two input polynomial.
A result handler is updated with each valid polinomial once it is input.

In order to plot the graph we create a vector array and assign x values by lerping from -10 to 10 in a number of even divisions, for each x value we lerp trough we evaluate the polynomial at that value and assign the y coordinate of the vector.
The array is then normalized with respect to the UI image serving as the xy coordinate system grid, such that on the x axis the function's x axis matches that of our image.
After the normalization all vectors with a y value out of bounds of the grid image are removed.
