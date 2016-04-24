<?php
/*
Name: 			Newsletter Subscribe
Written by: 	Okler Themes - (http://www.okler.net)
Version: 		4.3.1
*/

require_once('mailchimp/mailchimp.php');

// Step 1 - Set the apiKey - How get your Mailchimp API KEY - http://kb.mailchimp.com/article/where-can-i-find-my-api-key
$apiKey 	= 'b247c783bf7ce890293bb6e71d873c3f-us11';

// Step 2 - Set the listId - How to get your Mailchimp LIST ID - http://kb.mailchimp.com/article/how-can-i-find-my-list-id
$listId 	= 'ece131a9e8';

$MailChimp = new \Drewm\MailChimp($apiKey);

$result = $MailChimp->call('lists/subscribe', array(
                'id'                => $listId,
                'email'             => array('email'=>$_POST['email_address_footer']),
                'merge_vars'        => array('FNAME'=>'', 'LNAME'=>''), // Step 3 (Optional) - Vars - More Information - http://kb.mailchimp.com/merge-tags/using/getting-started-with-merge-tags
                'double_optin'      => false,
                'update_existing'   => false,
                'replace_interests' => false,
                'send_welcome'      => true,
            ));

if (in_array('error', $result)) {
    $arrResult = array ('response'=>'error','message'=>$result['error']);
	echo json_encode($arrResult);
} else {
    $arrResult = array ('response'=>'success');
	echo json_encode($arrResult);
	/*************** sendgrid *************************/



  //echo "<div class='alert-danger'><b>yeeeees</b></div>";
  
 /* 
 
  $from='info@sipma.ci';
  $to=$_POST['email'];
  
$url = 'https://api.sendgrid.com/';
$user = 'laressource';
$pass = 'Laressource@1';

$params = array(
    'api_user'  => $user,
    'api_key'   => $pass,
    'to'        => $to,
    'subject'   => 'Inscription Finance One Newsletter',
    'html'      => '<html>
  <head></head>
  <body>
  <div>
     <p style="color:#3399FF;font-size:18px;font-weight:bold;" align="center"> Finance One Newsletter</p><br>
	<img alt="Finance One" width="250" height="90" src="https://dl.dropboxusercontent.com/s/scltlwvretdfdr6/logo.png?dl=0" />			<br><br>	
       Merci d\'avoir souscrit a notre newsletter.<br>Nous vous informerons sur les nouvelles offres.

               
    </div>
  </body>
</html>
',
    'text'      => $message,
    'from'      => $from,
  );


$request =  $url.'api/mail.send.json';

// Generate curl request
$session = curl_init($request);
// Tell curl to use HTTP POST
curl_setopt ($session, CURLOPT_POST, true);
// Tell curl that this is the body of the POST
curl_setopt ($session, CURLOPT_POSTFIELDS, $params);
// Tell curl not to return headers, but do return the response
curl_setopt($session, CURLOPT_HEADER, false);
curl_setopt($session, CURLOPT_SSL_VERIFYPEER, false);
// Tell PHP not to use SSLv3 (instead opting for TLS)
//curl_setopt($session, CURLOPT_SSLVERSION, CURL_SSLVERSION_TLSv1_2);
curl_setopt($session, CURLOPT_RETURNTRANSFER, true);

// obtain response
$response = curl_exec($session);
curl_close($session);

// print everything out
print_r($response);

echo json_encode($arrResult);

*/


/******************************************************/
}






