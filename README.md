# vac-seen-generator
## What is this?
This repo is Part Four (of eight) of a workshop/activity/tutorial that comprises the "Vac-Seen System". This system is associated with, and specifically created for, the [Red Hat OpenShift Sandbox](https://developers.redhat.com/developer-sandbox).

At the end of this tutorial you will have an instance of a microservice that is running in an OpenShift cluster. The microservice will generate events into an Apache Kafka topic.

## Need help?
If you need help or get stuck, email devsandbox@redhat.com.
If you find a defect, [create an Issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/creating-an-issue) in this repository.

## Prerequisites
The following **three** prerequisites are necessary:
1. An account in [OpenShift Sandbox](https://developers.redhat.com/developer-sandbox) (No problem; it's free). This is not actually *necessary*, since you can use this tutorial with any OpenShift cluster.
1. The `oc` command-line tool for OpenShift. There are instructions later in this article for the installation of `oc`.
1. Your machine will need access to a command line; Bash or PowerShell, either is fine.

## All Operating Systems Welcome
You can use this activity regardless of whether your PC runs Windows, Linux, or macOS.

## Overview of microservice
This tutorial is structured so that a .NET developer can perform this workshop using free resources: the aformentioned Kafka cluster and [Openshift sandbox](https://developers.redhat.com/developer-sandbox). In addition, a few open source prerequesites are required.

The scenario is this: Field personnel report the administration of COVID-19 vaccinations. These vaccination events are sent to a Kafka cluster.

This particular repo references a microservice called "vac-seen-generator". It creates simulated vaccination events, which contain the following data:  
* RecipientID — string that identifies who got the shot. It's a GUID.
* EventTimestamp — date and time
* CountryCode — the hard-coded, two-character ISO code for the country. Currently "us".
* VaccinationType — string that denotes which vaccination, for example, Pfizer or Moderna.
* ShotNumber — an integer indicating which shot this is for this recipient, i.e. first, second, third, etc.
this repository.

## Using this microservice  
This microservice has an API endpoint to which you can post a date. For that date, a random number of random events (from 1 to 40) are added to the Kafka event stream. This API can be called multiple times; it uses the current date and time as the EventTimestamp property. Calling it over and over simply adds more events to the Kafka event stream.

This microservices depends on a Kafka instance and OpenShift Service Binding. Without those two dependencies, this program will do nothing.


## Overview of implementation  
The user:
1. Creates this vac-seen-generator app in their cluster using the "from Git" option.  
1. Makes available a managed Kafka instance.
1. Binds their managed Kafka cluster to the app using Service Binding.
1. Can now use this program.

## Part 1: Creating the application 
### Step 1.1
Run this command get pull the image from the image registry and create the OpenShift application:  

`oc new-app --name=vac-seen-generator --image=quay.io/donschenck/vac-seen-generator:latest`

## Conclusion
The microservice is now up and running.

\### End ###