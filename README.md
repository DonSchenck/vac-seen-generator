# vac-seen-generator

This source code, and [the associated tutorial](https://red.ht/csvax), are part of a ficticious demonstration that includes C# source code and [Red Hat OpenShift Streams for Apache Kafka](https://www.redhat.com/en/technologies/cloud-computing/openshift/openshift-streams-for-apache-kafka).

This tutorial is structured so that a .NET developer can perform this workshop using free resources: the aformentioned Kafka cluster and [Openshift sandbox](https://developers.redhat.com/developer-sandbox). In addition, a few open source prerequesites are required.

The scenario is this: Field personnel report the administration of COVID-19 vaccinations. These vaccination events are sent to a Kafka cluster.

This particular repo references a microservice called "vac-seen-generator". It creates simulated vaccination events, which contain the following data:  
* RecipientID — string that identifies who got the shot. It's a GUID.
* EventTimestamp — date and time
* CountryCode — the hard-coded, two-character ISO code for the country. Currently "us".
* VaccinationType — string that denotes which vaccination, for example, Pfizer or Moderna.
* ShotNumber — an integer indicating which shot this is for this recipient, i.e. first, second, third, etc.

## Need help?
If you need help or get stuck, email devsandbox@redhat.com.
If you find a defect, [create an Issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/creating-an-issue) in this repository.

## Using this microservice  
This microservice has an API endpoint to which you can post a date. For that date, a random number of random events (from 1 to 40) are added to the Kafka event stream. This API can be called multiple times; it uses the current date and time as the EventTimestamp property. Calling it over and over simply adds more events to the Kafka event stream.

This microservices depends on a Kafka instance and OpenShift Service Binding. Without those two dependencies, this program will do nothing.


## Overview of implementation  
The user:
1. Creates this vac-seen-generator app in their cluster using the "from Git" option.  
1. Makes available a managed Kafka instance.
1. Binds their managed Kafka cluster to the app using Service Binding.
1. Can now use this program.

## The result
The result of this tutorial is a Kafka topic loaded with vaccination events. A follow-on tutorial will, in turn, use that data.  

## About that CountryCode  
Why is CountryCode hard-coded?

In order to reinforce the concept of microservices — with the accent on "micro" — it was decided that every country would create their own reporting application. Another country may clone this repo and change the code to their own (e.g. "fr"), or they may use another programming language altogether.  

Additionally, it is expected that every country reports data to their own Kafka Topic, which is named according to their country code. For example, the United States (ISO country code "us") will write to the Kafka topic "us".

## The Tutorial for the entire system
[You can locate the tutorial for the entire system here](https://red.ht/csvax).

\### End ###