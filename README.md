## Overview

This is a rough extension of the [Sage ERP eBusiness Web Services API](http://www.sagemas.com/~/media/Company/ERP/White%20Papers/MAS_90_200_eBusiness_Web_Services_Spec.pdf), designed mainly to allow for the creation of new Postal codes within Sage ERP.

## Code Example

After installation within IIS, you can use your favorite SOAP client to insert postal codes. Example implementation in PHP:

```php
$client = @new SoapClient( $wsdl_endpoint, array( 'cache_wsdl' => 'WSDL_CACHE_NONE' ) );
$client->CreateZip( array(
														'PostCode'    => $postcode,
														'City'        => $city,
														'StateCode'   => $state,
														'CountryCode' => $country,
														"APIKey'      => $api_key
														)
												);

```
## Motivation

The Sage API works very well in creating / updating customers and sales orders. However, what if you need to create a customer with a postal code that doesn't exist in your system? Since the API provides no way to create a new postal code, you will not be able to create any customers or sales orders until you manually enter that postal code. This is a big oversight and makes the API essentially useless if you deal with a lot of customers outside of the United States.

## Installation

Use the provided code to create a WCF4 Service (.dll) project in Visual Studio, then add as a new application within IIS.

## API Reference

This API exposes one method, CreateZip, which takes an array of 4 parameters.
* PostCode, maximum 10 characters
* City, maximum 20 characters
* StateCode, maximum 2 characters
* CountryCode, maximum 2 characters
* API Key, hardcoded

Return is a SOAP Fault if the postal code already exists in the database, boolean TRUE if insertion was successful.

Parameters will be truncated to the field length limits before being inserted into the database. The API also checks to ensure the postal code doesn't exist before attempting to insert, however you should check this prior to invoking the API.

## Contributors

Ideally Sage will implement this same method in a future release of the API, but until then feel free to fork this repo and send a pull request for any changes.

