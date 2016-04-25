<?php


if(isset($_POST['email_address']) && isset($_POST['name']) && isset($_POST['message']) && isset($_POST['telephone']) )
{
  //echo "<div class='alert-danger'><b>yeeeees</b></div>";
  
  
  $email=htmlentities($_POST['email_address']);
  $name=htmlentities($_POST['name']);
  $message=htmlentities($_POST['message']);
  $telephone=htmlentities($_POST['telephone']);
  
  
 
  $from='laressource@live.fr';
  $to='laressource@live.fr';
  
$url = 'https://api.sendgrid.com/';
$user = 'laressource';
$pass = 'Laressource@1';

$params = array(
    'api_user'  => $user,
    'api_key'   => $pass,
    'to'        => $to,
    'subject'   => 'Nouveau message client',
    'html'      => 'Message posté par : '. $name. '<br /><br /> Email : '.$email.' <br /><br /> message	: '.$message.' <br /><br /> objet : '.$subject,
    'text'      => $message,
    'from'      => $email,
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
//print_r($response);

echo "<div class='alert-danger'><b>Votre requête a été prise en compte.Nous vous repondrons sous peu</b></div>";


}
else echo "<div class='alert-danger'><b>Vous êtes d&eacute;j&agrave; abonn&eacute; &agrave; notre newsletter.</b></div>";
?>