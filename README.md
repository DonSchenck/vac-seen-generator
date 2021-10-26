# vac-seen-generator

This is part of a ficticious demonstration that includes C# code, Apache Kafka, and [Red Hat OpenShift Streams for Apache Kafka](https://www.redhat.com/en/technologies/cloud-computing/openshift/openshift-streams-for-apache-kafka).

This tutorial is structured so that a .NET developer can perform this workshop using free resources: the aformentioned Kafka cluster and [Developer Sandbox for Red Hat OpenShift](https://developers.redhat.com/developer-sandbox). A few open source prerequesites are required.

The scenario is this: Field personnel report the issueance of COVID-19 vaccinations. These events are sent to a Kafka cluster.

The particular repo references a microservice called "vac-seen-generator". It creates simulated vaccination events, which contain the following data:
RecipientID string that identifies who got the shot. It's a GUID.
EventTimestamp date and time
CountryCode the hard-coded, two-character ISO code for the country. Currently "us".
VaccinationType string that denotes which vaccination, for example, Pfizer or Moderna.
ShotNumber an integer indicating which shot this is for this recipient, i.e. first, second, third, etc.

## Using this microservice  
This microservice is started by an OpenShift job. It can be started multiple times; it uses the current date and time as the EventTimestamp property. Running it over and over simply adds more events to the Kafka event stream.


## Overview of implementation  
The user:
1. Creates the app in their cluster using the "from Git" option.
1. Binds their managed Kafka cluster to the app.
1. Enjoys the magic.


## About that CountryCode  
Why is CountryCode hard-coded?

In order to reinforce the concept of microservices — with the accent on "micro" — it was decided that every country would create their own reporting application. Another country may clone this repo and change the code to their own (e.g. "fr"), or they may use another programming language altogether.  

Additionally, it is expected that every country reports data to their own Kafka Topic, which is named according to their country code. For example, the United States (ISO country code "us") will write to the Kafka topic "us".

## The Tutorial for the entire system
[You can locate the tutorial for the entire system here](https://red.ht/csvax).

\### End ###