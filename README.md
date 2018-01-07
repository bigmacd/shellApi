# Shell API

This is an "empty" Web API 2.0 project with some important features.

## This projects contains:

*  An example of Error Handling Middleware
*  An example of Audit Publishing using Kafka
*  An example usage of Statsd
*  An example Jenkinsfile
*  An example implementation of Swagger/Swashbuckle
*  An example Database Context

## Overview

## How to

### Error Handling

### Audit Publishing

### Statsd

### Jenkinsfile

### Swashbuckle/Swagger

### DbContext

While this project contains an example Database Context implementation, it leaves out a lot of detail as that detail is specific to any database schema.

An approach to use when your database already exists is to use the Scaffold-DbContext cmdlet:

```
PM> Scaffold-DbContext "server=mydbserver;Database=mydatabasename;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -t "tableName1", "tableName2","tableName3" -OutputDir Models -Context "ShellContext"
```

## Run & Deploy


