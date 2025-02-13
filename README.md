# in-com-coding-challange

	Hello, first thing I would like to say is that I am aware of a typo
in my repository name)
	
Regarding implementation:

	Firstly, I assumed that we don't need to store results in database, 
and decided that runtime static storage is enough. For clearing in memory 
"database" I added special button on the view called "Clear Database".

	Secondly, I implemented logic using **HashTable** data structure. 
But, probably, HashTable is slower than Dictionary because of a need to 
box and unbox object type.
	
	Thirdly, my txt and html parsers won't handle cases like "123Hello123" 
properly. In this particular case word "Hello" will be extracted and stored. 
And I am aware of that.