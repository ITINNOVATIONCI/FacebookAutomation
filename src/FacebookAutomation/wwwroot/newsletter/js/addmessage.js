$(document).ready(function() {
	$('#formAddMessage').click(function(){
	    var nom = $('#message-name').val();
	    var email = $('#message-email').val();
	    var phone = $('#message-phone').val();
	    var messg = $('#message-message').val();
		
		alert(nom);
	    $('#sendMessageStatus').hide("fast");
		var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
		if (!(filter.test(email))) {
		    $('#sendMessageStatus').show("fast");
			$('#sendMessageStatus').html("<h5>Email non valide</h5></div>");
			$('#sendMessageStatus').focus();
			//$('.error').remove();
//			$('.success').remove();
			return false;
		}
		//show the loading sign
        $('#loader').show();
		$.ajax({
			type: "post",
			url: "newsletter/sendgrid_mail.php",
			data: { email_address: email, message: messg, name: nom,telephone:phone},
			success: function(data)
			{
				alert(data);
				//$('#info').before(data);
				//$('#success').before(data);
				$('#loader').hide();
				
				$('#sendMessageStatus').html("<b>Votre requete a &eacute;t&eacute; prise en compte. Nous vous repondrons sous peu</b></div>");
				$('#sendMessageStatus').show(5000);
				$('#name').val("");
				$('#messg').val("");
				$('#email').val("");
				$('#phone').val("");
				
//				$('.warning').remove();
				
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
