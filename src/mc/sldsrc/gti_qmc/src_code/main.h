
#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>
#include <ctype.h>

typedef struct
{
	unsigned int length;
	bool has_been_compared;
	char* data; //KEIN STRING. ZAHLEN!
} char_array;

typedef struct
{
	unsigned int length;
	char_array** data; //KEIN STRING. ZAHLEN!
} char_array2d;

typedef struct node
{
	struct node* next;
	void* data;
} node;

typedef struct
{
	node* root;
	unsigned int length;
} list;

void* get_at(list* l, unsigned int i);
list* create_empty_list();
node* create_node(void* data);
void free_list(list* l);
void free_meta_list(list* meta_list);
void add_to_end(list* l, void* data);
void remove_at(list* l, unsigned int i);
void* pop(list* l, unsigned int i);
void remove_duplicates(list* l);
void remove_duplicates_meta(list* l);

char_array2d* create_empty_char_array2d(unsigned int width, unsigned int height);

unsigned int count_ones(char_array component);
void compare(list* current, list* next, list* meta_list, list* new_meta_list);
bool is_off_by_one_bit(char_array* currentComponent, char_array* nextComponent);
char_array* combine_components(char_array* currentComponent, char_array* nextComponent);
void wrap_it_up(bool is_done, list* new_meta_list, list* result_list);

bool a_covers_b(char_array* a, char_array* b);
void remove_column(list* meta_table, int index);
void remove_unimportant_rows_and_columns(list* meta_table, list* primeimplicant);
void remove_submissive_rows(list* meta_table, list* primeimplicant); //( ͡° ͜ʖ ͡°)
void remove_dominant_columns(list* meta_table, list* primeimplicant); //( ͡° ͜ʖ ͡°)

bool is_meta_table_empty(list* meta_table);
void choose_any_primeimplicant(list* meta_table, list* result, list* primeimplicants);

void collect_essentials(list* meta_table, list* result, list* primeimplicants);

list* convert_to_table(list* primeimplicants, char_array2d* minterms);
void add_to_meta_list_at(unsigned int index, list* base, char_array* data);

void simplify_function(char_array2d* args);
void phase_one(list* meta_list, list*);
list* phase_two(list* l, char_array2d* minterms);
bool parse_args(int argc, char** argv, char_array2d** values);

void print_char_array(char_array* arr);
void print_map(list*);
void print_map_debug(list* results);