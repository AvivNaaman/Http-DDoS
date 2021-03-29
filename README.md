# Http-DDoS
Some kind of DDoS Service &amp; Client, Written using C#

## Server ("Master")
Just a /Status endpoint which returns what to attack.
Maybe in the future a way to change it securely during run time will be added

## Client ("Endpoint")
A simple console application which starts a nice number of threads.
Each thread makes a GET request to the url provided by the /Status enpoint on the master.
The user can limit the number of threads, the wait interval and set the master endpoint url using command line arguments.
