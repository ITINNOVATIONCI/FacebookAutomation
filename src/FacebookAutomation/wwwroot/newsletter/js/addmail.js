$(document).ready(function() {
	$('#formNewLestter').click(function(){
		var email = $('.abhijitscript').val();
		//alert(email);
		$( '#info').hide( "fast" );
		var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
		if (!(filter.test(email))) {
			$( '#info').show( "fast" );
			$('#info').html("<h4>Email Invalide</h4></div>");
			$('#info').focus();
			//$('.error').remove();
//			$('.success').remove();
			return false;
		}
		//show the loading sign
        $('#loader').show();
		$.ajax({
			type: "post",
			url: "newsletter/add_mail.php",
			data: {email_address:email},
			success: function(data)
			{
				$('#info').before(data);
				//$('.abhijitscript').before(data);
//				$('.warning').remove();
$('#loader').hide();
				
			}
		});
		return false;
	});
	$( ".abhijitscript" ).keyup(function() {
	   var email = $('.abhijitscript').val();
	   if(email.length == 0)
	   {
		$('.error').remove();
		$('.success').remove();
		$('.warning').remove();
	   }
	});
});
