Console app that monitors a mailbox using POP3 and creates WorkItems in Team Foundation Server or Visual Studio Online. TFS is used in both cases.

# Usage

[![Join the chat at https://gitter.im/giuliov/Mail2WorkItem](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/giuliov/Mail2WorkItem?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

The tool has no command line option, configuration is taken from `Mail2WorkItem.ConsoleApp.exe.config` file.

It reads all available messages from the POP3 server and creates or update a TFS work item. An email is sent back to the original sender to inform of a successful operation.
When all POP3 messages are processed, the tool exits.

# Configuration

Configuration is organized in five areas:

 * General
 * POP3 connection
 * TFS connection and configuration
 * SMTP connection
 * Confirmation message templates

## General configuration
`LogLevel` can be:
 - Critical
 - Error
 - Warning
 - Information or Normal
 - Verbose

## POP3 connection data
POP3 connection data requires
 * Hostname
 * Port
 * UseSsl
 * Username
 * Password
All parameters are mandatory.

### TFS connection and configuration
TFS connection data requires
 * CollectionUrl -- e.g. `http://localhost:8080/tfs/DefaultCollection` or `https://`_youraccount_`.visualstudio.com/DefaultCollection/`
 * ProjectName -- where create the Work Items
 * WorkItemType -- the Type of work items to create

The Work Item `Title` is the POP3 mail Subject, the `Description` has the mail Body. The mail attachments become work item attachments.

## SMTP connection data
SMTP connection data requires
 * Hostname
 * Port
 * UseSsl
 * Username
 * Password
All parameters are mandatory.

## Confirmation message templates
 * Title -- the message Subject
 * Body -- the HTML message body
 * Sender -- the Sender Display Name

The templates use [SmartFormat.NET](https://github.com/scottrippey/SmartFormat.NET)

#TODO
[ ] return code
[ ] confirmation optional
